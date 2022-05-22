using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Akc.Saga.CosmosDb
{
    internal sealed class AzureCosmosOutboxListener : IHostedService, IDisposable
    {
        private readonly ISagaCommandPublisher _commandPublisher;
        private readonly CosmosClient _cosmosClient;
        private readonly ILogger<AzureCosmosOutboxListener> _logger;
        private readonly AkcSagaAzureCosmosOptions _cosmosOptions;
        private readonly AkcSagaConfiguration _configuration;
        private ChangeFeedProcessor? _changeProcessor;

        public AzureCosmosOutboxListener(
            ISagaCommandPublisher commandPublisher,
            IOptions<AkcSagaAzureCosmosOptions> cosmosOptions,
            AkcSagaConfiguration configuration,
            CosmosClient cosmosClient,
            ILogger<AzureCosmosOutboxListener> logger)
        {
            _commandPublisher = commandPublisher;
            _cosmosClient = cosmosClient;
            _logger = logger;
            _cosmosOptions = cosmosOptions.Value;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _changeProcessor = await StartChangeFeedProcessorAsync(_cosmosClient, _cosmosOptions, cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Change Feed Processor");

            if (_changeProcessor is not null)
            {
                await _changeProcessor.StopAsync();
            }
        }

        #region Private

        private async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync(
            CosmosClient cosmosClient,
            AkcSagaAzureCosmosOptions cosmosOptions,
            CancellationToken cancellationToken)
        {
            string databaseName = cosmosOptions.Database;
            string sourceContainerName = cosmosOptions.OutboxContainer;
            string leaseContainerName = "outbox-lease";

            var resp = await cosmosClient.GetDatabase(databaseName)
                    .CreateContainerIfNotExistsAsync(leaseContainerName, "/id", cancellationToken: cancellationToken);
            var leaseContainer = resp.Container;

            var changeFeedProcessor = cosmosClient.GetContainer(databaseName, sourceContainerName)
                .GetChangeFeedProcessorBuilder<CosmosDocumentEnveloppe>(processorName: nameof(AzureCosmosOutboxListener), onChangesDelegate: HandleChangesAsync)
                    .WithInstanceName("consoleHost")
                    .WithLeaseContainer(leaseContainer)
                    .Build();

            _logger.LogInformation("Starting Change Feed Processor");
            await changeFeedProcessor.StartAsync();
            _logger.LogInformation("Change Feed Processor started");

            return changeFeedProcessor;
        }

        private async Task HandleChangesAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<CosmosDocumentEnveloppe> changes, CancellationToken cancellationToken)
        {
            foreach (var item in changes)
            {
                var commandType = _configuration.NameToCommandType[item.TypeName];

                _logger.LogInformation("Change detected {CommandType}", commandType!.Name);

                var mem = new Memory<byte>(item!.Payload);
                using var jsonDocument = JsonDocument.Parse(mem);
                var command = (ISagaCommand?)jsonDocument.Deserialize(commandType, _cosmosOptions.PayloadSerializerOptions);

                await _commandPublisher.Publish(command!);
            }
        }

        public async void Dispose()
        {
            await (_changeProcessor?.StopAsync() ?? Task.CompletedTask);
        }

        #endregion
    }
}
