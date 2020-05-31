// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace EventGridFunction
{
    public static class EventGridFunctions
    {
        [FunctionName(nameof(ReceiveEventGridEvent))]
        public static void ReceiveEventGridEvent([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
        }

        [FunctionName(nameof(SendEventGridEvent))]
        public static async Task<IActionResult> SendEventGridEvent([HttpTrigger(AuthorizationLevel.Function, "post", 
            Route = "eventgrid")] HttpRequest req, ILogger log)
        {
            const string domainEndpoint = "https://azworkshop.eastus-1.eventgrid.azure.net/api/events";
            const string domainKey = "redacted=";

            var domainHostName = new Uri(domainEndpoint).Host;
            var topicCredentials = new TopicCredentials(domainKey);
            var client = new EventGridClient(topicCredentials);

            var eventGridEvents = new List<EventGridEvent>()
            {
                new EventGridEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = "azureWorkShop",
                    Data = new
                    {
                        Thing = "Thing1"
                    },
                    EventTime = DateTime.Now,
                    Subject = "azureWorkShop",
                    DataVersion = "2.0",
                    Topic = "/subscriptions/redacted/resourceGroups/azws/providers/Microsoft.EventGrid/domains/azworkshop/topics/azworkshoptopic1"
                }
            };

            try
            {
                await client.PublishEventsAsync(domainHostName, eventGridEvents);
            }
            catch (Exception e)
            {
               log.LogError(e.ToString());
               throw;
            }

            return new OkObjectResult("sent");
        }
    }
}
