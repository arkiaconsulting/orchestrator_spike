using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Akc.Saga
{
    internal class AzureServiceBusMessageBus : ISagaMessageBus
    {
        IEnumerable<ISagaCommand> ISagaMessageBus.Commands => _publishedCommands;
        private readonly ICollection<ISagaCommand> _publishedCommands = new List<ISagaCommand>();

        private readonly AzureServiceBusEntityLocator entityLocator;

        public AzureServiceBusMessageBus(AzureServiceBusEntityLocator entityLocator)
        {
            this.entityLocator = entityLocator;
        }

        async Task ISagaMessageBus.Publish<T>(T message)
        {
            var commandType = message.GetType();
            var sender = entityLocator.Locate(commandType);

            var data = new BinaryData(JsonSerializer.SerializeToUtf8Bytes(message, commandType));
            await sender.SendMessageAsync(new ServiceBusMessage(data)
            {
                Subject = message.Type
            });
        }
    }
}
