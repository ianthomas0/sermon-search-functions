
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Blackbaud.Church.PreachingCollective.Models;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace Blackbaud.Church.PreachingCollective
{
    public static class InsertSermon
    {
        private static string indexName = Indexes.SermonIndex;

        [FunctionName("InsertSermon")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            var searchAccessKey = System.Environment.GetEnvironmentVariable("SearchAccessKey", EnvironmentVariableTarget.Process);
            var searchService = System.Environment.GetEnvironmentVariable("SearchService", EnvironmentVariableTarget.Process);
            var searchCredentials = new SearchCredentials(searchAccessKey);
            var serviceClient = new SearchServiceClient(searchService, searchCredentials);

            var indexClient = serviceClient.Indexes.GetClient(indexName);

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var sermons = JsonConvert.DeserializeObject<IEnumerable<Sermon>>(requestBody);

            var batch = IndexBatch.Upload(sermons);

            indexClient.Documents.Index(batch);

            return new OkResult();
        }
    }
}
