using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace OrchestratedEventGridFunction
{
    public class SendEventToGridActivity
    {
        [FunctionName(nameof(SendEventGridEvent))]
        public async Task SendEventGridEvent([ActivityTrigger] IDurableActivityContext durableContext, ILogger log)
        {
            const string domainEndpoint = "https://azworkshop.eastus-1.eventgrid.azure.net/api/events";
            const string domainKey = "fP2AZuac49VpFCConZwqESmzhqybj5KSD0BqsYy3qPs=";
            
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
                        Thing = Guid.NewGuid()
                    },
                    EventTime = DateTime.Now,
                    Subject = "azureWorkShop",
                    DataVersion = "2.0",
                    Topic =
                        "/subscriptions/62db6d54-8749-47c6-b895-4877314dc60a/resourceGroups/azws/providers/Microsoft.EventGrid/domains/azworkshop/topics/azworkshoptopic1"
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
        }
    }
}
