using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttpGetFunction
{
    public static class HttpGetFunctions
    {
        [FunctionName(nameof(GetTheThings))]
        public static async Task<IActionResult> GetTheThings(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "things")] HttpRequest req,
            ILogger log)
        {
            //task delay for async warning
            await Task.Delay(1);
            
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            var things = new List<object> {new { ThingName = "thing1"}, new { ThingName = "thing2" } };
            return new OkObjectResult(things);
        }

        [FunctionName(nameof(GetTheThingsMoreGood))]
        public static async Task<IActionResult> GetTheThingsMoreGood(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "things/good")] HttpRequest req,
            ILogger log)
        {
            //task delay for async warning
            await Task.Delay(1);

            log.LogInformation("C# HTTP trigger function processed a request.");

            var hasKey = req.GetQueryParameterDictionary().TryGetValue("more", out var wantsMore);
            if (!hasKey || !string.Equals(wantsMore, "yes", StringComparison.InvariantCulture))
                return new BadRequestObjectResult(new {Reason = "more is not present in query string or yes was not selected"});
            
            var things = new List<object> { new { ThingName = "thing1" }, new { ThingName = "thing2" } };
            return new OkObjectResult(things);
        }
    }
}
