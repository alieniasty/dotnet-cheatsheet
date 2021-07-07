namespace AzureSamples.DeployAzFunctionFromCode
{
    public class ServiceConfig
    {
        public string AzureWebJobsStorage { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string TableStorageTableName { get; set; }
        public string PrincipalApplicationId { get; set; }
        public string PrincipalClientSecret { get; set; }
        public string PrincipalTenantId { get; set; }
        public string AzureSubscriptionId { get; set; }
        public string ResourceGroupName { get; set; }
        public string FunctionAppName { get; set; }
    }
}