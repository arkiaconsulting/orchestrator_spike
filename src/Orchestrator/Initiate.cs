using Akc.Saga;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Orchestrator.Saga;
using SharedLanguage;

namespace Orchestrator
{
    public class Initiate
    {
        private readonly SagaManager sagaManager;
        private readonly ILogger<Initiate> logger;

        public Initiate(
            SagaManager sagaManager,
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

            var sagaEvent = new InitiatedSagaEvent(command.TicketId);

            sagaManager.Handle<InvoiceDepositSaga, InitiatedSagaEvent>(command.TicketId.ToString(), sagaEvent);
        }
    }
}
