namespace AzureSamples.CosmosRepository
{
    using Microsoft.Azure.Cosmos;
    using System.Threading.Tasks;

    public class CosmosContainerFactory : ICosmosContainerFactory
    {
        private readonly CosmosClient _cosmosClient;
        private readonly CosmosDbConfig _config;
        private Database _database;
        private Container _collectionContainer;
        private Container _leasesContainer;

        public CosmosContainerFactory(CosmosClient cosmosClient, CosmosDbConfig config)
        {
            _cosmosClient = cosmosClient;
            _config = config;
        }

        public async Task<Container> GetContainer()
        {
            if (_collectionContainer == null)
            {
                await EnsureDatabase();

                if (_collectionContainer == null)
                {
                    _collectionContainer = await _database.CreateContainerIfNotExistsAsync(_config.CollectionContainerName, _config.CollectionPartitionKeyPath);
                }
            }

            return _collectionContainer;
        }

        public async Task<Container> GetLeasesContainer()
        {
            if (_leasesContainer == null)
            {
                await EnsureDatabase();

                if (_leasesContainer == null)
                {
                    _leasesContainer = await _database.CreateContainerIfNotExistsAsync(_config.LeasesContainerName, _config.LeasesPartitionKeyPath);
                }
            }

            return _leasesContainer;
        }

        private async Task EnsureDatabase()
        {
            if (_database == null)
            {
                _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_config.DatabaseName);
            }
        }
    }
}
