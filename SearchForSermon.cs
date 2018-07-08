
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
using System;
using System.Linq;

namespace Blackbaud.Church.PreachingCollective
{
    public static class SearchForSermon
    {
        [FunctionName("SearchForSermon")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, TraceWriter log)
        {
            var searchAccessKey = System.Environment.GetEnvironmentVariable("SearchAccessKey", EnvironmentVariableTarget.Process);
            var searchService = System.Environment.GetEnvironmentVariable("SearchService", EnvironmentVariableTarget.Process);
            var searchCredentials = new SearchCredentials(searchAccessKey);
            var serviceClient = new SearchServiceClient(searchService, searchCredentials);

            SearchParameters parameters;

            DocumentSearchResult<Sermon> results;

            parameters = new SearchParameters()
            {
                SearchFields = new[] { "Book" },
                Top = 10
            };

            var indexClient = serviceClient.Indexes.GetClient(Indexes.SermonIndex);

            results = indexClient.Documents.Search<Sermon>(req.Query["book"], parameters);

            var dedup = results.Results.GroupBy(x => x.Document.Title).Select(x => x.First()).Select(x => x.Document);

            return new OkObjectResult(dedup);
        }
    }
}
