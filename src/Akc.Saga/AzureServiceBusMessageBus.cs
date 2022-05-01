using Azure.Messaging.ServiceBus;

namespace Akc.Saga
{
    internal class AzureServiceBusMessageBus : ISagaMessageBus
    {
        private readonly AzureServiceBusEntityLocator entityLocator;

        public AzureServiceBusMessageBus(AzureServiceBusEntityLocator entityLocator)
        {
            this.entityLocator = entityLocator;
        }

        async Task ISagaMessageBus.Publish<T>(T message)
        {
            var sender = entityLocator.Locate<T>();

            await sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromObjectAsJson(message))
            {
                Subject = message.Type
            });
        }
    }
}
