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

        public ServiceBusSender Locate<T>()
        {
            return _senders.Single(s => s.EntityPath == _registrations[typeof(T)]);
        }
    }
}
