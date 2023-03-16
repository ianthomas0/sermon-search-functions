using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PreachingCollective.BusinessLogic;
using System.Threading.Tasks;
using PreachingCollective.Models;

namespace PreachingCollective
{
    public static class GetListFiltersData
    {
        [FunctionName("ListFiltersData")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:Remove unused parameter", Justification = "Meeting interface")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            var sermonsService = new SermonsService();

            var authors = await sermonsService.GetAuthors();
            var sources = await sermonsService.GetTopSources();

            var response = new ListFilterData
            {
                Authors = authors,
                Sources = sources
            };
            
            return new OkObjectResult(response);
        }
    }
}
