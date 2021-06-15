using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSamples.CosmosRepository
{
    public class CosmosDbConfig
    {
        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string DatabaseName { get; } = "db-name";

        public string CollectionContainerName { get; } = "Collection";

        public string LeasesContainerName { get; } = "Leases";

        public string CollectionPartitionKeyPath { get; } = "/id";

        public string LeasesPartitionKeyPath { get; } = "/id";
    }
}
