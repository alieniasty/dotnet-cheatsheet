namespace AzureSamples.CosmosRepository
{
    using System.Linq;
    using Microsoft.Azure.Cosmos;

    public interface ICosmosLinqQuery
    {
        FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query);
    }
}
