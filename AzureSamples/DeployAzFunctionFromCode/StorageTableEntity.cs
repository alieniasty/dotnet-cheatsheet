namespace AzureSamples.DeployAzFunctionFromCode
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class StorageTableEntity : TableEntity
    {
        public string AppName { get; set; }
        public string Description { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string ApiKey { get; set; }
        public string Topic { get; set; }
    }
}