using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Akc.Saga.AzureServiceBus
{
    internal class AzureServiceBusCommandPublisher : ISagaCommandPublisher
    {
        private readonly AzureServiceBusEntityLocator entityLocator;

        public AzureServiceBusCommandPublisher(AzureServiceBusEntityLocator entityLocator)
        {
            this.entityLocator = entityLocator;
        }

        async Task ISagaCommandPublisher.Publish<T>(T command)
        {
            var sender = entityLocator.Locate(command);

            var data = new BinaryData(JsonSerializer.SerializeToUtf8Bytes(command, command.GetType()));
            await sender.SendMessageAsync(new ServiceBusMessage(data)
            {
                Subject = command.Type
            });
        }
    }
}
