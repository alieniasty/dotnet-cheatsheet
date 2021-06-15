using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSamples.CosmosRepository
{
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Linq;
    using Microsoft.VisualBasic;

    public class CosmosRepository
    {
        private readonly ICosmosContainerFactory _cosmosContainerFactory;
        private readonly ICosmosLinqQuery _cosmosLinqQuery;

        public CosmosRepository(ICosmosContainerFactory cosmosContainerFactory, ICosmosLinqQuery cosmosLinqQuery)
        {
            _cosmosContainerFactory = cosmosContainerFactory;
            _cosmosLinqQuery = cosmosLinqQuery;
        }

        public async Task Create(Entity entity)
        {
            var container = await _cosmosContainerFactory.GetContainer();

            try
            {
                entity.Id = Guid.NewGuid().ToString();
                entity.Version = 1;

                await container.CreateItemAsync(entity);
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
                {
                    throw new Exception();
                }

                throw;
            }
        }

        public async Task<Entity> GetById(string id)
        {
            var container = await _cosmosContainerFactory.GetContainer();

            try
            {
                var response = await container.ReadItemAsync<Entity>(id, new PartitionKey(id));

                if (response.Resource is null) throw new Exception();
                return response.Resource;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception();
                }

                throw;
            }
        }

        public async Task<Entity> GetByCoreProperty(string key, string value, bool nullIfNotFound = false)
        {
            var entityType = typeof(Entity);
            var coreProperty = entityType.GetProperty(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (coreProperty == null)
            {
                throw new Exception();
            }

            var exprParameter = Expression.Parameter(entityType);
            var equalsMethod = typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) });

            // Below translates to "stringequals" cosmos db function for case in-sensitive search
            var exprCorePropertyComparison = Expression.Call(Expression.Property(exprParameter, coreProperty), equalsMethod, Expression.Constant(value), Expression.Constant(StringComparison.OrdinalIgnoreCase));

            var expr = Expression.Lambda<Func<Entity, bool>>(exprCorePropertyComparison, exprParameter);

            return await GetSingleByFilter(expr);
        }

        public async Task Update(Entity entity)
        {
            var container = await _cosmosContainerFactory.GetContainer();
            try
            {
                entity.Version++;
                await container.ReplaceItemAsync(entity, entity.Id, new PartitionKey(entity.Id), new ItemRequestOptions { IfMatchEtag = entity.ETag });
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    throw new Exception("Data has changed since your last request.", e);
                }

                throw;
            }
        }

        public async Task<bool> RemoveEntity(string id)
        {
            try
            {
                var container = await _cosmosContainerFactory.GetContainer();
                await container.DeleteItemAsync<Entity>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException)
            {
                return false;
            }
        }

        public async Task<Entity> GetSingleByQuery(string sqlQuery)
        {
            var container = await _cosmosContainerFactory.GetContainer();
            var query = container.GetItemQueryIterator<Entity>(sqlQuery);

            var resource = (await query.ReadNextAsync()).FirstOrDefault();

            return resource;
        }

        private async Task<Entity> GetSingleByFilter(Expression<Func<Entity, bool>> filter)
        {
            var container = await _cosmosContainerFactory.GetContainer();
            var query = container.GetItemLinqQueryable<Entity>().Where(filter);

            var count = await query.CountAsync();

            if (count == 0)
            {
                throw new Exception();
            }

            if (count > 1)
            {
                throw new Exception("Multiple results");
            }

            using (var iterator = _cosmosLinqQuery.GetFeedIterator(query))
            {
                if (!iterator.HasMoreResults)
                {
                    throw new Exception();
                }

                var resource = (await iterator.ReadNextAsync()).FirstOrDefault();

                return resource;
            }
        }

        public async Task<List<Entity>> GetManyByFilter(Expression<Func<Entity, bool>> filter)
        {
            var container = await _cosmosContainerFactory.GetContainer();
            var query = container.GetItemLinqQueryable<Entity>().Where(filter);

            if (await query.CountAsync() == 0)
            {
                throw new Exception();
            }

            using (var iterator = _cosmosLinqQuery.GetFeedIterator(query))
            {
                var resource = (await iterator.ReadNextAsync()).ToList();

                return resource;
            }
        }

    }
}
