using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Akc.Saga.CosmosDb
{
    internal class AzureCosmosDbEventStore : ISagaEventStore
    {
        private readonly Container _container;
        private readonly CosmosSerializer _serializer;
        private readonly ILogger<AzureCosmosDbEventStore> _logger;
        private readonly AkcSagaAzureCosmosOptions _options;

        public AzureCosmosDbEventStore(
            EventStoreContainer eventStoreContainer,
            IOptions<AkcSagaAzureCosmosOptions> options,
            ILogger<AzureCosmosDbEventStore> logger)
        {
            _container = eventStoreContainer.Container;
            _serializer = _container.Database.Client.ClientOptions.Serializer;
            _logger = logger;
            _options = options.Value;
        }

        async Task<IEnumerable<ISagaEvent>> ISagaEventStore.Load(string sagaId)
        {
            _logger.LogInformation("Loading saga {SagaId}", sagaId);

            var qDef = new QueryDefinition("SELECT * FROM c where c.sagaId = @sagaId")
                .WithParameter("@sagaId", sagaId);

            var it = _container.GetItemQueryIterator<CosmosSagaEvent>(qDef);
            var events = new List<(string TypeName, string Payload)>();
            do
            {
                var resp = await it.ReadNextAsync();
                events.AddRange(resp.OrderBy(e => e.CreatedAt).Select(e => (e.Type, e.Payload)));
            } while (it.HasMoreResults);

            return events.Select(Deserialize);
        }

        async Task ISagaEventStore.Save<TEvent>(string sagaId, TEvent @event)
        {
            var eventTypeName = _options.EventTypeToName[@event.GetType()];

            _logger.LogInformation("Saving a new saga {SagaId} event {EventType}", sagaId, eventTypeName);

            using var eStream = _serializer.ToStream(@event);
            using var sr = new StreamReader(eStream);

            var envelope = new CosmosSagaEvent(Guid.NewGuid().ToString(), sagaId, eventTypeName, sr.ReadToEnd(), DateTimeOffset.UtcNow);

            using var stream = _serializer.ToStream(envelope);
            var resp = await _container.CreateItemStreamAsync(stream, new PartitionKey(sagaId));

            resp.EnsureSuccessStatusCode();
        }

        #region Private

        private ISagaEvent Deserialize((string TypeName, string Payload) rawEvent)
        {
            var eventType = _options.NameToEventType[rawEvent.TypeName];

            return (ISagaEvent)JsonSerializer.Deserialize(rawEvent.Payload, eventType)!;
        }

        #endregion
    }
}
