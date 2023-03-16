
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using PreachingCollective.Models;
using System;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace PreachingCollective
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
            var searchCredentials = new AzureKeyCredential(searchAccessKey);
            var serviceClient = new SearchIndexClient(new Uri(searchService), searchCredentials);

            var idField = new SimpleField("id", SearchFieldDataType.String);
            var ridField = new SimpleField("rid", SearchFieldDataType.String) {
                IsKey = true
            };

            var fields = new FieldBuilder().Build(typeof(Sermon));
            fields.Add(idField);
            fields.Add(ridField);

            serviceClient.CreateOrUpdateIndex(new SearchIndex(indexName)
            {
                Fields = fields
            });

            return new OkResult();
        }
    }
}
