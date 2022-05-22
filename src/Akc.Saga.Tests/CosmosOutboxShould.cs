using System.Linq;
using System.Threading.Tasks;
using Akc.Saga.InMemory;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Akc.Saga.Tests
{
    [Trait("Category", "OutOfProcess")]
    public class CosmosOutboxShould : IAsyncLifetime
    {
        private SagaHost SagaHost => _host.Services.GetRequiredService<SagaHost>();
        private readonly IHost _host;

        public CosmosOutboxShould()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddAkcSaga(cfg =>
                    {
                        cfg.Register<MyOrderWorkflow, OrderCreated>(e => e.OrderId);
                        cfg.Register<MyOrderWorkflow, PaymentReceived>(e => e.OrderId);
                        cfg.RegisterEvents(typeof(OrderCreated).Assembly);
                        cfg.RegisterCommands(typeof(CreateOrder).Assembly);
                    })
                    .AddTestCosmos(ctx.Configuration)
                    .AddAkcSagaAzureCosmosOutbox();
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
            var commandPublisher = _host.Services.GetRequiredService<ISagaCommandPublisher>() as InMemorySagaCommandPublisher;

            while (!commandPublisher!.Commands.Any())
            {
            }

            commandPublisher!.Commands.Should().Contain(command);
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            await _host.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        #endregion
    }
}
