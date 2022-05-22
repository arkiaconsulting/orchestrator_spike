using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Akc.Saga.CosmosDb
{
    internal class AzureCosmosDbEventStore : ISagaEventStore
    {
        private readonly Container _container;
        private readonly CosmosSerializer _serializer;
        private readonly ILogger<AzureCosmosDbEventStore> _logger;
        private readonly AkcSagaConfiguration _configuration;

        public AzureCosmosDbEventStore(
            EventStoreContainer eventStoreContainer,
            AkcSagaConfiguration configuration,
            ILogger<AzureCosmosDbEventStore> logger)
        {
            _container = eventStoreContainer.Container;
            _serializer = _container.Database.Client.ClientOptions.Serializer;
            _logger = logger;
            _configuration = configuration;
        }

        async Task<IEnumerable<ISagaEvent>> ISagaEventStore.Load(string sagaId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Loading saga {SagaId}", sagaId);

            var qDef = new QueryDefinition("SELECT * FROM c where c.sagaId = @sagaId")
                .WithParameter("@sagaId", sagaId);

            var events = new List<(DateTimeOffset CreatedAt, string TypeName, string Payload)>();
            using var feedIterator = _container.GetItemQueryIterator<CosmosSagaEvent>(qDef);
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<CosmosSagaEvent> response = await feedIterator.ReadNextAsync(cancellationToken);
                foreach (var item in response.Resource)
                {
                    events.Add((item.CreatedAt, item.Type, item.Payload));
                }
            }

            return events.OrderBy(e => e.CreatedAt).Select(Deserialize);
        }

        async Task ISagaEventStore.Save<TEvent>(string sagaId, TEvent @event, CancellationToken cancellationToken)
        {
            var eventTypeName = _configuration.EventTypeToName[@event.GetType()];

            _logger.LogInformation("Saving a new saga {SagaId} event {EventType}", sagaId, eventTypeName);

            using var eStream = _serializer.ToStream(@event);
            using var sr = new StreamReader(eStream);

            var envelope = new CosmosSagaEvent(Guid.NewGuid().ToString(), sagaId, eventTypeName, sr.ReadToEnd(), DateTimeOffset.UtcNow);

            using var stream = _serializer.ToStream(envelope);
            var resp = await _container.CreateItemStreamAsync(stream, new PartitionKey(sagaId), cancellationToken: cancellationToken);

            resp.EnsureSuccessStatusCode();
        }

        #region Private

        private ISagaEvent Deserialize((DateTimeOffset CreatedAt, string TypeName, string Payload) rawEvent)
        {
            var eventType = _configuration.NameToEventType[rawEvent.TypeName];

            return (ISagaEvent)JsonSerializer.Deserialize(rawEvent.Payload, eventType)!;
        }

        #endregion
    }
}
