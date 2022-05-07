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
        private ISagaOutbox Outbox => _host.Services.GetRequiredService<ISagaOutbox>();
        private ISagaMessageBus MessageBus => _host.Services.GetRequiredService<ISagaMessageBus>();

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
                    });
                })
                .Build();
        }

        [Theory(DisplayName = "Create the order when the buyer creates it")]
        [AutoData]
        public void Test01(string orderId)
        {
            SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId));

            AssertCommandPublished(new CreateOrder(orderId));
        }

        [Theory(DisplayName = "Not ship the goods if the order hasn't been created")]
        [AutoData]
        public void Test02(string orderId)
        {
            SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(new PaymentReceived(orderId));

            AssertNoCommandProduced();
        }

        [Theory(DisplayName = "Ship the goods when the payment is received")]
        [AutoData]
        public void Test03(string orderId)
        {
            SagaHost.Handle<MyOrderWorkflow, OrderCreated>(new OrderCreated(orderId));

            SagaHost.Handle<MyOrderWorkflow, PaymentReceived>(new PaymentReceived(orderId));

            AssertCommandPublished(new ShipOrder(orderId));
        }

        #region Private

        private void AssertNoCommandProduced()
        {
            Outbox.Commands.Should().BeEmpty();
        }

        private void AssertCommandPublished<T>(T command) where T : ISagaCommand
        {
            MessageBus.Commands.Should().Contain(command);
        }

        #endregion
    }

    internal record OrderCreated(string OrderId) : ISagaEvent;
    internal record PaymentReceived(string OrderId) : ISagaEvent;
    internal record CreateOrder(string OrderId) : ISagaCommand
    {
        public string Type { get; } = nameof(CreateOrder);
    }

    internal record ShipOrder(string OrderId) : ISagaCommand
    {
        public string Type { get; } = nameof(ShipOrder);
    }

    internal class MyOrderWorkflow : Saga,
        ISagaEventHandler<OrderCreated>,
        ISagaEventHandler<PaymentReceived>
    {
        private bool _created = false;

        public void Handle(OrderCreated @event, IMessageContext context)
        {
            _created = true;

            Publish(new CreateOrder(@event.OrderId), context);
        }

        public void Handle(PaymentReceived @event, IMessageContext context)
        {
            if (_created)
            {
                Publish(new ShipOrder(@event.OrderId), context);
            }
        }
    }
}