using System.Threading.Tasks;
using Akc.Saga.InMemory;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Akc.Saga.Tests
{
    [Trait("Category", "Acceptance")]
    public class SagaShould
    {
        private SagaHost SagaHost => _host.Services.GetRequiredService<SagaHost>();
        private ISagaCommandOutbox Outbox => _host.Services.GetRequiredService<ISagaCommandOutbox>();
        private ISagaCommandPublisher MessageBus => _host.Services.GetRequiredService<ISagaCommandPublisher>();

        private readonly IHost _host;

        public SagaShould()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAkcSaga(cfg =>
                    {
                        cfg.Register<MyOrderWorkflow, OrderCreated>(e => e.OrderId);
                        cfg.Register<MyOrderWorkflow, PaymentReceived>(e => e.OrderId);
                        cfg.RegisterEvents(typeof(OrderCreated).Assembly);
                        cfg.RegisterCommands(typeof(CreateOrder).Assembly);
                    });
                })
                .Build();
        }

        [Theory(DisplayName = "Create the order when the buyer creates it")]
        [AutoData]
        public async Task Test01(string orderId)
        {
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId));

            AssertCommandPublished(new CreateOrder(orderId));
        }

        [Theory(DisplayName = "Not ship the goods if the order hasn't been created")]
        [AutoData]
        public async Task Test02(string orderId)
        {
            await SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(new PaymentReceived(orderId));

            AssertNoCommandProduced();
        }

        [Theory(DisplayName = "Ship the goods when the payment is received")]
        [AutoData]
        public async Task Test03(string orderId)
        {
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId));

            await SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(new PaymentReceived(orderId));

            AssertCommandPublished(new ShipOrder(orderId));
        }

        [Theory(DisplayName = "Not ship the goods of another order")]
        [AutoData]
        public async Task Test04(string orderId1, string orderId2)
        {
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId1));
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId2));

            await SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(new PaymentReceived(orderId1));

            AssertCommandNotPublished(new ShipOrder(orderId2));
        }

        [Theory(DisplayName = "Not ship a second time once the saga has already completed")]
        [AutoData]
        public async Task Test05(string orderId)
        {
            await SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId));

            await SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(new PaymentReceived(orderId));
            await SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(new PaymentReceived(orderId));

            AssertCommandPublishedSingleTime(new ShipOrder(orderId));
        }

        #region Private

        private void AssertNoCommandProduced()
        {
            (Outbox as InMemorySagaCommandOutbox)!.Commands.Should().BeEmpty();
        }

        private void AssertCommandPublished<T>(T command) where T : ISagaCommand
        {
            (MessageBus as InMemorySagaCommandPublisher)!.Commands.Should().Contain(command);
        }

        private void AssertCommandNotPublished<T>(T command) where T : ISagaCommand
        {
            (MessageBus as InMemorySagaCommandPublisher)!.Commands.Should().NotContain(command);
        }

        private void AssertCommandPublishedSingleTime<T>(T command) where T : class, ISagaCommand
        {
            (MessageBus as InMemorySagaCommandPublisher)!.Commands
                .Should().ContainSingle(c => c is T && (T)c == command);
        }

        #endregion
    }
}