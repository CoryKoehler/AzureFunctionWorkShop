using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace OrchestratedEventGridFunction
{
    public class OrchestrationFunction
    {
        [FunctionName(nameof(Orchestrate))]
        public async Task Orchestrate([OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext, ILogger log)
        {
            await Task.WhenAll(
                orchestrationContext.CallSubOrchestratorAsync(nameof(Orchestrate100EventGridEvents), new { }),
                orchestrationContext.CallSubOrchestratorAsync(nameof(Orchestrate100EventGridEvents), new { })
            );
        }

        [FunctionName(nameof(Orchestrate100EventGridEvents))]
        public async Task Orchestrate100EventGridEvents(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext,
            ILogger log)
        {
            var parallelTasks = new List<Task>();
            const int eventsToSend = 100;

            for (var i = 0; eventsToSend >= i; i++)
            {
                var activity =
                    orchestrationContext.CallActivityAsync(nameof(SendEventToGridActivity.SendEventGridEvent), new {});
                parallelTasks.Add(activity);
            }

            await Task.WhenAll(parallelTasks);
        }
    }
}
