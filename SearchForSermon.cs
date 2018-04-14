
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

namespace Blackbaud.Church.PreachingCollective
{
    public static class SearchForSermon
    {
        [FunctionName("SearchForSermon")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            var searchCredentials = new SearchCredentials("0FD59A5E379EDBF58D9E283D47DB9683");
            var serviceClient = new SearchServiceClient("preaching-collective", searchCredentials);

            SearchParameters parameters;

            DocumentSearchResult<Sermon> results;

            parameters = new SearchParameters()
            {
                SearchFields = new[] { "Book" },
                Top = 10
            };

            var indexClient = serviceClient.Indexes.GetClient(Indexes.SermonIndex);

            results = indexClient.Documents.Search<Sermon>(req.Query["book"]);

            return new OkObjectResult(results.Results);
        }
    }
}
