using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedLanguage;

namespace BusinessWorker
{
    public class SecurityCheck
    {
        private readonly OrchestratorSender sender;
        private readonly ILogger<SecurityCheck> logger;

        public SecurityCheck(
            OrchestratorSender sender,
            ILogger<SecurityCheck> logger)
        {
            this.sender = sender;
            this.logger = logger;
        }

        [Function("SecurityCheck")]
        // https://github.com/Azure/azure-sdk-for-net/issues/21884
        //[ServiceBusOutput("orchestrator", Connection = "SBConnectionString")]
        public async Task Run(
            [ServiceBusTrigger("security-check", Connection = "SBConnectionString")] CheckSecurityCommand command,
            string correlationId)
        {
            logger.LogInformation($"Consuming {nameof(CheckSecurityCommand)} {{TicketId}}", command.TicketId);

            logger.LogInformation($"Publishing {nameof(SecurityCheckedEvent)} {{TicketId}}", command.TicketId);

            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new SecurityCheckedEvent(command.TicketId)))
            {
                Subject = nameof(SecurityCheckedEvent),
                CorrelationId = correlationId
            };
            await sender.SendMessageAsync(message);
        }
    }
}
