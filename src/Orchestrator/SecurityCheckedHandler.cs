using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedLanguage;

namespace Orchestrator
{
    public class SecurityCheckedHandler
    {
        private readonly DispatchSender sender;
        private readonly ILogger<SecurityCheckedEvent> logger;

        public SecurityCheckedHandler(
            DispatchSender sender,
            ILogger<SecurityCheckedEvent> logger)
        {
            this.sender = sender;
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

            logger.LogInformation($"Publishing {nameof(DispatchCommand)} {{TicketId}}", @event.TicketId);

            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new DispatchCommand(@event.TicketId)))
            {
                Subject = nameof(DispatchCommand),
                CorrelationId = correlationId
            };
            await sender.SendMessageAsync(message);
        }
    }
}
