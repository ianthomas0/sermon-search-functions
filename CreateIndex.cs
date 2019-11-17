
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Search;
using Blackbaud.Church.PreachingCollective.Models;
using System;
using Microsoft.Extensions.Logging;

namespace Blackbaud.Church.PreachingCollective
{
    public static class CreateIndex
    {
        private static string indexName = Indexes.SermonIndex;

        [FunctionName("CreateIndex")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Remove unused parameter", Justification = "Meeting interface")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            var searchAccessKey = System.Environment.GetEnvironmentVariable("SearchAccessKey", EnvironmentVariableTarget.Process);
            var searchService = System.Environment.GetEnvironmentVariable("SearchService", EnvironmentVariableTarget.Process);
            var searchCredentials = new SearchCredentials(searchAccessKey);
            var serviceClient = new SearchServiceClient(searchService, searchCredentials);

            serviceClient.Indexes.GetClient(indexName);

            if (!serviceClient.Indexes.Exists(indexName))
            {
                var definition = new Microsoft.Azure.Search.Models.Index()
                {
                    Name = indexName,
                    Fields = FieldBuilder.BuildForType<Sermon>()
                };

                serviceClient.Indexes.Create(definition);
            }

            serviceClient.Dispose();

            return new OkResult();
        }
    }
}
