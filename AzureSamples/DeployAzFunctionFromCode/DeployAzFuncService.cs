namespace AzureSamples.DeployAzFunctionFromCode
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus.Administration;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class DeployAzFuncService
    {
        private readonly ServiceConfig _config;
        private readonly CloudTable _cloudTable;
        private readonly ServiceBusAdministrationClient _serviceBusAdminClient;
        private readonly IAzure _azure;
        private readonly ILogger<DeployAzFuncService> _log;

        public DeployAzFuncService(ServiceConfig config, ILogger<DeployAzFuncService> log)
        {
            _config = config;
            _log = log;

            var storageAccount = CloudStorageAccount.Parse(config.AzureWebJobsStorage);
            _cloudTable = storageAccount.CreateCloudTableClient().GetTableReference(config.TableStorageTableName);

            _serviceBusAdminClient = new ServiceBusAdministrationClient(config.ServiceBusConnectionString);

            var servicePrincipalApplicationId = config.PrincipalApplicationId;
            var servicePrincipalClientSecret = config.PrincipalClientSecret;
            var servicePrincipalTenantId = config.PrincipalTenantId;
            var azureSubscriptionId = config.AzureSubscriptionId;
            var creds = SdkContext.AzureCredentialsFactory.FromServicePrincipal(servicePrincipalApplicationId, servicePrincipalClientSecret, servicePrincipalTenantId, AzureEnvironment.AzureGlobalCloud);
            
            _azure = Azure.Authenticate(creds).WithSubscription(azureSubscriptionId);
        }

        public async Task CreateRowInTableAndDeploy(StorageTableEntity entity)
        {
            await CreateStorageTableRow(entity);
            await CreateServiceBusSubscription(entity);
            await DeployAzureFunction(entity);
        }

        public async Task DeleteRowFromTableAndDestroy(string appName, string topic)
        {
            var entity = await GetStorageTableEntry(appName, topic);

            await DeleteAzureFunction(entity);
            await DeleteServiceBusSubscriber(entity);
            await DeleteStorageTableRow(entity);
        }
        
        public async Task RedeployAllAppRegistrations()
        {
            var allRegistrations = await GetAllStorageTableEntries();

            foreach (var registration in allRegistrations)
            {
                await DeleteAzureFunction(registration);
                await DeployAzureFunction(registration);
            }
        }

        private async Task<IList<StorageTableEntity>> GetAllStorageTableEntries()
        {
            var query = new TableQuery<StorageTableEntity>();
            var tableQueryResult = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);
            var listOfRegisteredApps = tableQueryResult.ToList();
            
            return listOfRegisteredApps;
        }

        private async Task<StorageTableEntity> GetStorageTableEntry(string appName, string topic)
        {
            var retrieveOperation = TableOperation.Retrieve<StorageTableEntity>(appName, topic);
                
            var result = await _cloudTable.ExecuteAsync(retrieveOperation);
            var entity = result.Result as StorageTableEntity;
            return entity;
        }

        private async Task CreateStorageTableRow(ITableEntity entity)
        {
            await _cloudTable.CreateIfNotExistsAsync();
            var insertOperation = TableOperation.Insert(entity);
            await _cloudTable.ExecuteAsync(insertOperation);
        }

        private async Task DeleteStorageTableRow(ITableEntity entity)
        {
            var deleteOperation = TableOperation.Delete(entity);
            await _cloudTable.ExecuteAsync(deleteOperation);
        }

        private async Task CreateServiceBusSubscription(StorageTableEntity entity)
        {
            var topicName = entity.Topic;
            var subscriptionName = entity.AppName;

            var options = new CreateSubscriptionOptions(topicName, subscriptionName)
            {
                DeadLetteringOnMessageExpiration = true,
                DefaultMessageTimeToLive = new TimeSpan(0, 5, 0, 0),
                EnableDeadLetteringOnFilterEvaluationExceptions = false,
                MaxDeliveryCount = 5,
            };
            
            await _serviceBusAdminClient.CreateSubscriptionAsync(options);
        }

        private async Task DeleteServiceBusSubscriber(StorageTableEntity entity)
        {
            var topicName = entity.Topic;
            var subscriptionName = entity.AppName;

            await _serviceBusAdminClient.DeleteSubscriptionAsync(topicName, subscriptionName);
        }

        private async Task DeployAzureFunction(StorageTableEntity entity)
        {
            const string deploymentName = "webhook-function-deployment";
            
            var resourceGroupName = _config.ResourceGroupName;
            var functionAppName = _config.FunctionAppName;
            var topicName = entity.Topic;
            var subscriptionName = entity.AppName;

            var armTemplate = await ReadArmTemplate(entity, topicName, subscriptionName, functionAppName, subscriptionName);

            await _azure.Deployments
                .Define(deploymentName)
                .WithExistingResourceGroup(resourceGroupName)
                .WithTemplate(armTemplate)
                .WithParameters(new object())
                .WithMode(Microsoft.Azure.Management.ResourceManager.Fluent.Models.DeploymentMode.Incremental)
                .CreateAsync();
        }

        private async Task DeleteAzureFunction(StorageTableEntity entity)
        {
            var resourceGroupName = _config.ResourceGroupName;
            var functionAppName = _config.FunctionAppName;
            var subscriptionName = entity.AppName;
            
            await _azure.GenericResources.DeleteAsync(resourceGroupName, "Microsoft.Web/sites", functionAppName, "functions", subscriptionName, "2020-09-01");
        }

        private async Task<string> ReadArmTemplate(StorageTableEntity entity, string serviceBusTopic, string serviceBusTopicSubscription, string azureFunctionApp, string azureFunction)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourcePath = "AzureSamples.DeployAzFunctionFromCode.FunctionTemplate.arm-template.json";

            using var stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream);
            var template = await reader.ReadToEndAsync();

            template = template
                .Replace("{{function-app-name}}", azureFunctionApp)
                .Replace("{{function-name}}", azureFunction)
                .Replace("{{run-csx-text}}", await ReadRunCsx(entity))
                .Replace("\"{{function-binding-json}}\"", await ReadFunctionBinding(serviceBusTopic, serviceBusTopicSubscription));

            _log.LogDebug(template);

            return template;
        }

        private async Task<string> ReadRunCsx(StorageTableEntity entity)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            const string runCsxPath = "AzureSamples.DeployAzFunctionFromCode.FunctionTemplate.run.csx";

            using var stream = assembly.GetManifestResourceStream(runCsxPath);
            using var reader = new StreamReader(stream);
            var runCsxText = await reader.ReadToEndAsync();

            var appName = entity.AppName;
            var endpoint = entity.Endpoint;
            var method = entity.Method.ToUpperInvariant();
            var apiKey = entity.ApiKey;

            runCsxText = runCsxText
                .Replace("{{app-name}}", appName)
                .Replace("{{endpoint}}", endpoint)
                .Replace("{{method}}", method)
                .Replace("{{api-key}}", apiKey)
                .Replace("\\", "\\\\")
                .Replace("\r\n", "\\r\\n")
                .Replace("\"", "\\\"");

            _log.LogDebug(runCsxText);

            return runCsxText;
        }

        private async Task<string> ReadFunctionBinding(string serviceBusTopic, string serviceBusTopicSubscription)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string functionJsonPath = "UserRole.AppIntegrations.Functions.ServiceBusToWebhookTemplate.function.json";

            await using var stream = assembly.GetManifestResourceStream(functionJsonPath);
            using var reader = new StreamReader(stream);
            var functionJson = await reader.ReadToEndAsync();

            functionJson = functionJson
                .Replace("{{topic-name}}", serviceBusTopic)
                .Replace("{{subscription-name}}", serviceBusTopicSubscription);

            _log.LogDebug(functionJson);

            return functionJson;
        }
    }
}