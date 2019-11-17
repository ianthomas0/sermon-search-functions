
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Blackbaud.Church.PreachingCollective.Models;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;

namespace Blackbaud.Church.PreachingCollective
{
    public static class InsertSermon
    {
        private static string _indexName = Indexes.SermonIndex;

        [FunctionName("InsertSermon")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            if (req is null)
            {
                return new BadRequestObjectResult("Request body can not be null");
            }

            // Get search service credentials
            var searchAccessKey = System.Environment.GetEnvironmentVariable("SearchAccessKey", EnvironmentVariableTarget.Process);
            var searchService = System.Environment.GetEnvironmentVariable("SearchService", EnvironmentVariableTarget.Process);
            var searchCredentials = new SearchCredentials(searchAccessKey);

            // Get search service client and grab the index
            var serviceClient = new SearchServiceClient(searchService, searchCredentials);
            var indexClient = serviceClient.Indexes.GetClient(_indexName);
            serviceClient.Dispose();

            var streamReader = new StreamReader(req.Body);
            string requestBody = streamReader.ReadToEnd();
            streamReader.Dispose();

            var sermons = JsonConvert.DeserializeObject<IEnumerable<Sermon>>(requestBody);
            var batch = IndexBatch.Upload(sermons);
            indexClient.Documents.Index(batch);
            log.LogInformation($"Uploaded {sermons.Count()} sermons from {sermons.First().Source}");


            // Store the sermons as documents in blob storage
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pcsermonfiles;AccountKey=pOz/+tEOuxqpGMEnmvKEhDZ/HlX+Kp85fTpNmHvIJj2zgVu3p9wXUt5FoRAojc2dimNL3TaoSGB/3WX9qHL8Rg==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount;
            var parsed = CloudStorageAccount.TryParse(storageConnectionString, out storageAccount);

            if (parsed)
            {
                // Create the CloudBlobClient that represents the 
                // Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and 
                // append a GUID value to it to make the name unique.
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("sermons");
                cloudBlobContainer.CreateIfNotExistsAsync();

                foreach (var sermon in sermons)
                {
                    // Get a reference to the blob address, then upload the file to the blob.
                    // Use the value of localFileName for the blob name.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"{sermon.Id}.json");
                    cloudBlockBlob.UploadTextAsync(JsonConvert.SerializeObject(sermon, Formatting.Indented));
                }
            }

            return new OkResult();
        }
    }
}
