using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Blackbaud.Church.PreachingCollective.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Blackbaud.Church.PreachingCollective
{
    public static class SearchForSermon
    {
        [FunctionName("SearchForSermon")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Remove unused parameter", Justification = "Meeting interface")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            if (req is null)
            {
                return new BadRequestObjectResult("Request body can not be null");
            }

            var searchAccessKey = System.Environment.GetEnvironmentVariable("SearchAccessKey", EnvironmentVariableTarget.Process);
            var searchService = System.Environment.GetEnvironmentVariable("SearchService", EnvironmentVariableTarget.Process);
            var searchCredentials = new SearchCredentials(searchAccessKey);
            var serviceClient = new SearchServiceClient(searchService, searchCredentials);

            var pageSize = 1000;
            var book = req.Query["book"];
            var chapter = req.Query["chapter"];
            var source = req.Query["source"];

            var parameters = new SearchParameters()
            {
                Top = pageSize,
                SearchFields = new List<string> { "Title" },
                OrderBy = new List<string> { "Book asc", "Chapter asc", "VerseStart asc" }
            };

            string filter = "";

            // Add chapter and verse filtering, if book filter is present
            if (!string.IsNullOrEmpty(book))
            {
                filter = $"Book eq '{book}'";

                if (!string.IsNullOrWhiteSpace(chapter))
                {
                    filter = $"{filter} and Chapter eq {chapter}";
                }
            }

            // Add source filtering
            if (!string.IsNullOrWhiteSpace(source))
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    filter = $"{filter} and ";
                }

                filter = $"{filter}Source eq '{source}'";
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                parameters.Filter = filter;
            }

            if (!string.IsNullOrEmpty(req.Query["page"]))
            {
                parameters.Skip = (int.Parse(req.Query["page"], CultureInfo.InvariantCulture) - 1) * pageSize;
            }

            var indexClient = serviceClient.Indexes.GetClient(Indexes.SermonIndex);

            var results = indexClient.Documents.Search<Sermon>(req.Query["search"], parameters);

            var dedup = results.Results.GroupBy(x => x.Document.Title).Select(x => x.First()).Select(x => x.Document);

            serviceClient.Dispose();

            return new OkObjectResult(dedup);
        }
    }
}
