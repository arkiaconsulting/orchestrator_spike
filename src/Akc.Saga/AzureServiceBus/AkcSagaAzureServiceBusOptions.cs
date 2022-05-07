namespace Akc.Saga.AzureServiceBus
{
    public class AkcSagaAzureServiceBusOptions
    {
        public IReadOnlyDictionary<Type, string> Registrations => _registrations;

        private readonly Dictionary<Type, string> _registrations = new();

        public void RegisterMessageEntity<T>(string entityPath) where T : ISagaCommand
        {
            _registrations.Add(typeof(T), entityPath);
        }
    }
}