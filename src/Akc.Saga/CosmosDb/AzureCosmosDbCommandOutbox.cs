using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Azure.Cosmos;

namespace Akc.Saga.CosmosDb
{
    internal class AzureCosmosDbCommandOutbox : ISagaCommandOutbox
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly Container container;

        public AzureCosmosDbCommandOutbox(OutboxContainer container)
        {
            this.container = container.Container;
        }

        async Task ISagaCommandOutbox.Publish<T>(T command)
        {
            using var ms = container.Database.Client.ClientOptions.Serializer.ToStream(command);

            var node = JsonNode.Parse(ms)!;
            var id = Guid.NewGuid().ToString();
            node.AsObject().Add("id", id);

            using var dest = new MemoryStream();
            using var jw = new Utf8JsonWriter(dest, new() { Indented = false });
            node.WriteTo(jw, _options);
            jw.Flush();

            _ = await container.CreateItemStreamAsync(dest, new PartitionKey(id));
        }
    }
}
