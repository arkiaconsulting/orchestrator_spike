using Akc.Saga;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Orchestrator.Saga;
using SharedLanguage;

namespace Orchestrator
{
    public class Initiate
    {
        private readonly SagaHost sagaManager;
        private readonly ILogger<Initiate> logger;

        public Initiate(
            SagaHost sagaManager,
            ILogger<Initiate> logger)
        {
            this.sagaManager = sagaManager;

            this.logger = logger;
        }

        [Function("Initiate")]
        // https://github.com/Azure/azure-sdk-for-net/issues/21884
        //[ServiceBusOutput("security-check", Connection = "SBConnectionString")]
        public async Task Run(
            [ServiceBusTrigger("initiate", Connection = "SBConnectionString")] InitiateCommand command,
            string correlationId)
        {
            logger.LogInformation($"Consuming {nameof(InitiateCommand)} {{TicketId}}", command.TicketId);

            var sagaEvent = new InitiatedSagaEvent(command.TicketId.ToString());

            await sagaManager.Handle<InvoiceDepositSaga, InitiatedSagaEvent>(sagaEvent);
        }
    }
}
