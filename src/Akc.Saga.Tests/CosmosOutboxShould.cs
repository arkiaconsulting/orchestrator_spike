using System.Linq;
using System.Threading.Tasks;
using Akc.Saga.CosmosDb;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Akc.Saga.Tests
{
    [Trait("Category", "OutOfProcess")]
    public class CosmosOutboxShould
    {
        private SagaHost SagaHost => _host.Services.GetRequiredService<SagaHost>();
        private readonly IHost _host;
        private const string ConnectionString = "AccountEndpoint=https://localhost:8081;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string Database = "orch-spike";
        private const string Container = "tests-outbox";

        public CosmosOutboxShould(ITestOutputHelper testOutputHelper)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddAkcSaga(cfg =>
                    {
                        cfg.Register<MyOrderWorkflow, OrderCreated>(e => e.OrderId);
                        cfg.Register<MyOrderWorkflow, PaymentReceived>(e => e.OrderId);
                    });
                    services.AddAkcSagaAzureCosmosDb(ctx.Configuration,
                        config =>
                        {
                            config.ConnectionString = ConnectionString;
                            config.Database = Database;
                            config.OutboxContainer = Container;
                            config.PreferMsi = false;
                        }, builder =>
                        {
                            builder.WithConnectionModeGateway().WithLimitToEndpoint(true);
                        });
                })
                .Build();
        }

        [Theory(DisplayName = "Add the produced command to the outbox")]
        [AutoData]
        public async Task Test01(string orderId)
        {
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId));

            AssertCommandPublished(new CreateOrder(orderId));
        }

        #region Private

        private void AssertCommandPublished(CreateOrder command)
        {
            var container = _host.Services.GetRequiredService<OutboxContainer>().Container;

            var items = container.GetItemLinqQueryable<CreateOrder>(allowSynchronousQueryExecution: true,
                linqSerializerOptions: new() { PropertyNamingPolicy = Microsoft.Azure.Cosmos.CosmosPropertyNamingPolicy.CamelCase })
                .Where(cmd => cmd.OrderId == command.OrderId)
                .AsEnumerable();

            items.Should().ContainSingle();
        }

        #endregion
    }
}
