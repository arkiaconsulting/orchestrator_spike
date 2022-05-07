using Akc.Saga;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Orchestrator.Saga;
using SharedLanguage;

namespace Orchestrator
{
    public class DispatchedHandler
    {
        private readonly SagaHost sagaManager;
        private readonly ILogger<DispatchedHandler> logger;

        public DispatchedHandler(
            SagaHost sagaManager,
            ILogger<DispatchedHandler> logger)
        {
            this.sagaManager = sagaManager;

            this.logger = logger;
        }

        [Function("DispatchedHandler")]
        public void Run([ServiceBusTrigger("orchestrator", "dispatched", Connection = "SBConnectionString")] DispatchedEvent @event)
        {
            logger.LogInformation($"Handling {nameof(DispatchedEvent)} {{TicketId}}", @event.TicketId);

            logger.LogInformation($"Workflow ended {{TicketId}}", @event.TicketId);

            var sagaEvent = new DispatchedSagaEvent(@event.TicketId.ToString());

            sagaManager.Handle<InvoiceDepositSaga, DispatchedSagaEvent>(sagaEvent);
        }
    }
}
