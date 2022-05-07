using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Akc.Saga
{
    internal class AzureServiceBusEntityLocator
    {
        private readonly IReadOnlyDictionary<Type, string> _registrations;
        private readonly IEnumerable<ServiceBusSender> _senders;

        public AzureServiceBusEntityLocator(
            IEnumerable<ServiceBusSender> senders,
            IOptions<AkcSagaAzureServiceBusOptions> options)
        {
            _registrations = options.Value.Registrations;
            _senders = senders;
        }

        public ServiceBusSender Locate(Type commandType)
        {
            var registered = _registrations.TryGetValue(commandType, out var registeredServiceBusEntity);
            if (!registered)
            {
                throw new InvalidOperationException($"Unable to locate any Service Bus registration for the given type '{commandType.Name}'");
            }

            var registeredSender = _senders.FirstOrDefault(s => s.EntityPath == registeredServiceBusEntity);
            if (registeredSender is null)
            {
                throw new InvalidOperationException($"No sender was found for the Service Bus entity '{registeredServiceBusEntity}'");
            }

            return registeredSender;
        }
    }
}
