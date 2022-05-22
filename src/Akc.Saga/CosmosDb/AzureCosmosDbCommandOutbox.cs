using System.Buffers;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Akc.Saga.CosmosDb
{
    internal class AzureCosmosDbCommandOutbox : ISagaCommandOutbox
    {
        private readonly Container _container;
        private readonly AkcSagaConfiguration _configuration;
        private readonly JsonSerializerOptions _serializerOptions;

        public AzureCosmosDbCommandOutbox(
            OutboxContainer container,
            AkcSagaConfiguration configuration,
            IOptions<AkcSagaAzureCosmosOptions> options)
        {
            _container = container.Container;
            _configuration = configuration;
            _serializerOptions = options.Value.PayloadSerializerOptions;
        }

        async Task ISagaCommandOutbox.Publish<T>(T command)
        {
            var buffer = new ArrayBufferWriter<byte>();
            using var sw = new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(sw, command, command.GetType(), _serializerOptions);
            await sw.FlushAsync();

            var commandTypeName = _configuration.CommandTypeToName[command.GetType()];
            var document = new CosmosDocumentEnveloppe(Guid.NewGuid().ToString(), commandTypeName, buffer.WrittenSpan.ToArray());

            using var ms = _container.Database.Client.ClientOptions.Serializer.ToStream(document);

            _ = await _container.CreateItemStreamAsync(ms, new PartitionKey(document.Id));
        }
    }
}
