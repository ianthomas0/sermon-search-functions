
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
            var searchCredentials = new SearchCredentials("1159A4B94EF6E53D4F10A8C7F1FDD9DD");
            var serviceClient = new SearchServiceClient("preachingcollective", searchCredentials);

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
