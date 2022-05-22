using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Akc.Saga.AzureServiceBus
{
    internal class AzureServiceBusCommandPublisher : ISagaCommandPublisher
    {
        private readonly AzureServiceBusEntityLocator entityLocator;
        private readonly AkcSagaConfiguration _configuration;

        public AzureServiceBusCommandPublisher(
            AzureServiceBusEntityLocator entityLocator,
            AkcSagaConfiguration configuration)
        {
            this.entityLocator = entityLocator;
            _configuration = configuration;
        }

        async Task ISagaCommandPublisher.Publish<T>(T command)
        {
            var sender = entityLocator.Locate(command);

            var eventTypeName = _configuration.CommandTypeToName[command.GetType()];

            var data = new BinaryData(JsonSerializer.SerializeToUtf8Bytes(command, command.GetType()));
            await sender.SendMessageAsync(new ServiceBusMessage(data)
            {
                Subject = eventTypeName
            });
        }
    }
}
