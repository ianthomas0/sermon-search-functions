
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Blackbaud.Church.PreachingCollective.Models;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using PreachingCollective.BusinessLogic;

namespace Blackbaud.Church.PreachingCollective
{
    /// <summary>
    /// Insert sermons as documents into blob storage to be indexed for azure search
    /// </summary>
    public static class InsertSermon
    {
        [FunctionName("InsertSermon")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            if (req is null)
            {
                return new BadRequestObjectResult("Request body can not be null");
            }

            var streamReader = new StreamReader(req.Body);
            string requestBody = streamReader.ReadToEnd();
            streamReader.Dispose();

            var sermons = JsonConvert.DeserializeObject<IEnumerable<SermonInsert>>(requestBody);

            var sermonsService = new SermonsService();

            foreach (var sermon in sermons)
            {
                await sermonsService.UpsertSermon(sermon);
            }

            log.LogInformation($"Uploaded {sermons.Count()} sermons from {sermons.First().Source}");

            return new OkResult();
        }
    }
}
