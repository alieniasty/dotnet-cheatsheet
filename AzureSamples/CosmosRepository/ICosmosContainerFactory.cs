namespace AzureSamples.CosmosRepository
{
    using Microsoft.Azure.Cosmos;
    using System.Threading.Tasks;

    public interface ICosmosContainerFactory
    {
        Task<Container> GetContainer();

        Task<Container> GetLeasesContainer();
    }
}
