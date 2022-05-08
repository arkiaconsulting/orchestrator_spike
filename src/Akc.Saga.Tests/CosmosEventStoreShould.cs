using System.Linq;
using System.Threading.Tasks;
using Akc.Saga.CosmosDb;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Akc.Saga.Tests
{
    [Trait("Category", "OutOfProcess")]
    public class CosmosEventStoreShould
    {
        private SagaHost SagaHost => _host.Services.GetRequiredService<SagaHost>();
        private readonly IHost _host;

        public CosmosEventStoreShould()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddAkcSaga(cfg =>
                    {
                        cfg.Register<MyOrderWorkflow, OrderCreated>(e => e.OrderId);
                        cfg.Register<MyOrderWorkflow, PaymentReceived>(e => e.OrderId);
                    })
                    .AddTestCosmos(ctx.Configuration)
                    .AddAkcSagaAzureCosmosEventStore();
                })
                .Build();
        }

        [Theory(DisplayName = "Handle the order creation")]
        [AutoData]
        internal async Task Test01(OrderCreated order)
        {
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(order);

            AssertEventStored(order, order.OrderId);
        }

        [Theory(DisplayName = "Can ship the order")]
        [AutoData]
        internal async Task Test02(OrderCreated order)
        {
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(order);

            var payment = new PaymentReceived(order.OrderId);
            await SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(payment);

            AssertEventStored(order, order.OrderId);
            AssertEventStored(payment, order.OrderId);
        }

        #region Private

        private void AssertEventStored<T>(T @event, string orderId) where T : ISagaEvent
        {
            var container = _host.Services.GetRequiredService<EventStoreContainer>().Container;

            var items = container.GetItemLinqQueryable<CosmosSagaEvent>(allowSynchronousQueryExecution: true,
                linqSerializerOptions: new() { PropertyNamingPolicy = Microsoft.Azure.Cosmos.CosmosPropertyNamingPolicy.CamelCase })
                .Where(e => e.SagaId == orderId && e.Type == TypeHelpers.GetEventTypeName(@event.GetType()))
                .AsEnumerable();

            items.Should().ContainSingle();
        }

        #endregion
    }
}
