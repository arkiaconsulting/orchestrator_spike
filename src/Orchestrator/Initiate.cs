using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedLanguage;

namespace Orchestrator
{
    public class Initiate
    {
        private readonly SecurityCheckSender sender;
        private readonly ILogger<Initiate> logger;

        public Initiate(
            SecurityCheckSender sender,
            ILogger<Initiate> logger)
        {
            this.sender = sender;
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

            logger.LogInformation($"Publishing {nameof(CheckSecurityCommand)} {{TicketId}}", command.TicketId);

            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new CheckSecurityCommand(command.TicketId)))
            {
                Subject = nameof(CheckSecurityCommand),
                CorrelationId = correlationId
            };
            await sender.SendMessageAsync(message);
        }
    }
}
