using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedLanguage;

namespace BusinessWorker
{
    public class Dispatch
    {
        private readonly OrchestratorSender sender;
        private readonly ILogger<Dispatch> logger;

        public Dispatch(
            OrchestratorSender sender,
            ILogger<Dispatch> logger)
        {
            this.sender = sender;
            this.logger = logger;
        }

        [Function("Dispatch")]
        // https://github.com/Azure/azure-sdk-for-net/issues/21884
        //[ServiceBusOutput("orchestrator", Connection = "SBConnectionString")]
        public async Task Run([ServiceBusTrigger("dispatch", Connection = "SBConnectionString")] DispatchCommand command,
            string correlationId)
        {
            logger.LogInformation($"Consuming {nameof(DispatchCommand)} {{TicketId}}", command.TicketId);

            logger.LogInformation($"Publishing {nameof(DispatchedEvent)} {{TicketId}}", command.TicketId);

            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new DispatchedEvent(command.TicketId)))
            {
                Subject = nameof(DispatchedEvent),
                CorrelationId = correlationId
            };
            await sender.SendMessageAsync(message);
        }
    }
}
