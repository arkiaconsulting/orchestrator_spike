using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedLanguage;

namespace Orchestrator
{
    public class DispatchedHandler
    {
        private readonly ILogger<DispatchedHandler> logger;

        public DispatchedHandler(ILogger<DispatchedHandler> logger)
        {
            this.logger = logger;
        }

        [Function("DispatchedHandler")]
        public void Run([ServiceBusTrigger("orchestrator", "dispatched", Connection = "SBConnectionString")] DispatchedEvent @event)
        {
            logger.LogInformation($"Handling {nameof(DispatchedEvent)} {{TicketId}}", @event.TicketId);

            logger.LogInformation($"Workflow ended {{TicketId}}", @event.TicketId);
        }
    }
}
