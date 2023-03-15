using Blackbaud.Church.PreachingCollective.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PreachingCollective.BusinessLogic
{
    internal class SermonsService
    {
        private Container _container;

        internal SermonsService() { }

        public async Task<IEnumerable<string>> GetAuthors()
        {
            var container = await GetContainer();
            var sqlQueryText = "SELECT DISTINCT(c.Author) FROM c";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<string> queryResultSetIterator = container.GetItemQueryIterator<string>(queryDefinition);

            List<string> authors = new List<string>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<string> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                authors.AddRange(currentResultSet);
            }

            return authors;
        }

        public async Task<IEnumerable<string>> GetTopSources()
        {
            var container = await GetContainer();
            var sqlQueryText = "SELECT TOP 20 VALUE COUNT(1) AS Count, c.Source FROM c GROUP BY c.Source ORDER BY Count DESC";

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
