
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
    /// <summary>
    /// Insert sermons as documents into blob storage to be indexed for azure search
    /// </summary>
    public static class InsertSermon
    {
        [FunctionName("InsertSermon")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            if (req is null)
            {
                return new BadRequestObjectResult("Request body can not be null");
            }

            var streamReader = new StreamReader(req.Body);
            string requestBody = streamReader.ReadToEnd();
            streamReader.Dispose();

            var sermons = JsonConvert.DeserializeObject<IEnumerable<Sermon>>(requestBody);

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

            log.LogInformation($"Uploaded {sermons.Count()} sermons from {sermons.First().Source}");

            return new OkResult();
        }
    }
}
