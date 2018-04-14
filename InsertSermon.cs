
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

namespace Blackbaud.Church.PreachingCollective
{
    public static class InsertSermon
    {
        private static string indexName = Indexes.SermonIndex;

        [FunctionName("InsertSermon")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            var searchCredentials = new SearchCredentials("0FD59A5E379EDBF58D9E283D47DB9683");
            var serviceClient = new SearchServiceClient("preaching-collective", searchCredentials);

            var indexClient = serviceClient.Indexes.GetClient(indexName);


            if (!serviceClient.Indexes.Exists(indexName))
            {
                var definition = new Index()
                {
                    Name = indexName,
                    Fields = FieldBuilder.BuildForType<Sermon>()
                };

                serviceClient.Indexes.Create(definition);
            }

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            Sermon data = JsonConvert.DeserializeObject<Sermon>(requestBody);
            data.Id = Guid.NewGuid().ToString();

            var sermons = new List<Sermon>()
            {
                data
            };

            var batch = IndexBatch.Upload(sermons);

            indexClient.Documents.Index(batch);

            return new OkResult();
        }
    }
}
