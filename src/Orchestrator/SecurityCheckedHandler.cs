using Akc.Saga;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Orchestrator.Saga;
using SharedLanguage;

namespace Orchestrator
{
    public class SecurityCheckedHandler
    {
        private readonly SagaHost sagaManager;
        private readonly ILogger<SecurityCheckedEvent> logger;

        public SecurityCheckedHandler(
            SagaHost sagaManager,
            ILogger<SecurityCheckedEvent> logger)
        {
            this.sagaManager = sagaManager;
            this.logger = logger;
        }

        [Function("SecurityCheckedHandler")]
        // https://github.com/Azure/azure-sdk-for-net/issues/21884
        //[ServiceBusOutput("dispatch", Connection = "SBConnectionString")]
        public async Task Run(
            [ServiceBusTrigger("orchestrator", "security-checked", Connection = "SBConnectionString")] SecurityCheckedEvent @event,
            string correlationId)
        {
            logger.LogInformation($"Handling {nameof(SecurityCheckedEvent)} {{TicketId}}", @event.TicketId);

            var sagaEvent = new SecurityCheckedSagaEvent(@event.TicketId.ToString());

            await sagaManager.Handle<InvoiceDepositSaga, SecurityCheckedSagaEvent>(sagaEvent).ConfigureAwait(false);
        }
    }
}
