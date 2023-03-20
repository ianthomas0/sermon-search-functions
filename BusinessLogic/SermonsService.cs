using PreachingCollective.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PreachingCollective.BusinessLogic
{
    internal class SermonsService
    {
        private Container _container;

        internal SermonsService() { }

        public async Task<IEnumerable<string>> GetAuthors()
        {
            var container = await GetContainer();
            var sqlQueryText = "SELECT COUNT(c.Author) as Count, c.Author FROM c GROUP BY c.Author";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var queryRequestOptions = new QueryRequestOptions
            {
                MaxItemCount = 500
            };

            FeedIterator<AuthorsQueryResult> queryResultSetIterator = container.GetItemQueryIterator<AuthorsQueryResult>(queryDefinition, requestOptions: queryRequestOptions);

            List<AuthorsQueryResult> authors = new List<AuthorsQueryResult>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<AuthorsQueryResult> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                authors.AddRange(currentResultSet);
            }

            var sortedAuthors = authors.OrderByDescending(a => a.Count).ToList();

            return sortedAuthors.Take(20).OrderBy(a => a.Author).Select(a => a.Author);
        }

        public async Task<IEnumerable<string>> GetTopSources()
        {
            var container = await GetContainer();
            var sqlQueryText = "SELECT DISTINCT VALUE c.Source FROM c";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<string> queryResultSetIterator = container.GetItemQueryIterator<string>(queryDefinition);

            List<string> sources = new List<string>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<string> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                sources.AddRange(currentResultSet);
            }

            return sources;
        }

        public async Task UpsertSermon(SermonInsert sermon)
        {
            var container = await GetContainer();

            await container.UpsertItemAsync(sermon);
        }

        public async Task DeleteDocument(string id)
        {
            var container = await GetContainer();

            await container.DeleteItemAsync<Sermon>(id, new PartitionKey(id));
        }

        private async Task<Container> GetContainer()
        {
            if (_container == null)
            {
                var cosmosEndpoint = Environment.GetEnvironmentVariable("CosmosEndpoint", EnvironmentVariableTarget.Process);
                var cosmosKey = Environment.GetEnvironmentVariable("CosmosAccessKey", EnvironmentVariableTarget.Process);
                var cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);

                var database = await cosmosClient.CreateDatabaseIfNotExistsAsync("sermons");
                var container = await database.Database.CreateContainerIfNotExistsAsync("sermon", "/id");

                _container = container.Container;
            }

            return _container;
        }
    }
}
