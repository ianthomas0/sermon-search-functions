using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Azure.Search.Documents;
using PreachingCollective.Models;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Options;

namespace PreachingCollective
{
    public static class SermonsList
    {
        [FunctionName("SermonsList")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Remove unused parameter", Justification = "Meeting interface")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            if (req is null)
            {
                return new BadRequestObjectResult("Request body can not be null");
            }

            var searchAccessKey = Environment.GetEnvironmentVariable("SearchAccessKey", EnvironmentVariableTarget.Process);
            var searchService = Environment.GetEnvironmentVariable("SearchService", EnvironmentVariableTarget.Process);
            var searchCredentials = new AzureKeyCredential(searchAccessKey);
            var serviceClient = new SearchIndexClient(new Uri(searchService), searchCredentials);

            // Define parameters from query string
            var pageSize = 1000;
            var chapter = req.Query["chapter"];
            var chapterEnd = req.Query["chapterEnd"];
            var verseStart = req.Query["verseStart"];
            var source = req.Query["source"];
            var author = req.Query["author"];
            var bookOrder = req.Query["bookNum"];

            var parameters = new SearchOptions()
            {
                Size = pageSize
            };

            parameters.SearchFields.Add("Title");
            parameters.OrderBy.Add("BookOrder asc");
            parameters.OrderBy.Add("Chapter asc");
            parameters.OrderBy.Add("VerseStart asc");

            string filter = "";

            if(string.IsNullOrEmpty(bookOrder))
            {
                filter = $"BookOrder ne null";
            }

            // Add chapter and verse filtering, if book filter is present
            // Must account for whole chapter references
            if (!string.IsNullOrEmpty(bookOrder))
            {
                filter = $"BookOrder eq {bookOrder}";

                // Filter by chapter, finding chapter references WITHIN range
                // start chapter of REFERENCE <= start chapter of SEARCH
                // end chapter of REFERENCE >= end chapter of SEARCH
                if (!string.IsNullOrWhiteSpace(chapter))
                {
                    if (string.IsNullOrWhiteSpace(chapterEnd))
                    {
                        filter = $"{filter} and Chapter le {chapter} and ChapterEnd ge {chapter}";
                    }
                    else
                    {
                        filter = $"{filter} and ChapterEnd ge {chapter} and Chapter le {chapterEnd}";
                    }
                }

                // Filter by chapter, finding chapter references WITHIN range
                // start chapter of REFERENCE <= start chapter of SEARCH
                // end chapter of REFERENCE >= end chapter of SEARCH
                if (!string.IsNullOrWhiteSpace(verseStart))
                {
                    filter = $"{filter} and VerseStart ge {verseStart}";
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

            // Add author filtering
            if (!string.IsNullOrWhiteSpace(author))
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    filter = $"{filter} and ";
                }

                filter = $"{filter}Author eq '{author}'";
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                parameters.Filter = filter;
            }

            if (!string.IsNullOrEmpty(req.Query["page"]))
            {
                parameters.Skip = (int.Parse(req.Query["page"], CultureInfo.InvariantCulture) - 1) * pageSize;
            }

            var indexClient = serviceClient.GetSearchClient(Indexes.SermonIndex);

            var search = indexClient.Search<Sermon>(req.Query["search"], parameters);
            var results = search.Value.GetResults();
            var dedup = results.GroupBy(x => x.Document.Title).Select(x => x.First()).Select(x => x.Document);

            return new OkObjectResult(dedup);
        }
    }
}
