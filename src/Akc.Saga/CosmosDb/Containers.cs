using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Akc.Saga.CosmosDb
{
    internal class OutboxContainer
    {
        public Container Container { get; private set; }

        public OutboxContainer(CosmosClient cosmosClient, IOptions<AkcSagaAzureCosmosOptions> options)
        {
            Container = cosmosClient.GetContainer(options.Value.Database, options.Value.OutboxContainer);
        }
    }
}
