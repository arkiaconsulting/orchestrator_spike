using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Akc.Saga.Tests
{
    [Trait("Category", "Acceptance")]
    public class SagaShould
    {
        private SagaManager MySagaManager
        {
            get
            {
                var sm = new SagaManager(_eventStore, _outbox);
                sm.Register(rootId => new MyOrderWorkflow(rootId));

                return sm;
            }
        }
        private readonly ISagaEventStore _eventStore = new InMemorySagaEventStore();
        private readonly ISagaOutbox _outbox = new InMemorySagaOutbox();

        [Theory(DisplayName = "Create the order when the buyer creates it")]
        [AutoData]
        public void Test01(string orderId)
        {
            MySagaManager.Handle<MyOrderWorkflow, OrderCreated>(orderId, new OrderCreated(orderId));

            AssertCommandProduced(new CreateOrder(orderId));
        }

        [Theory(DisplayName = "Not ship the goods if the order hasn't been created")]
        [AutoData]
        public void Test02(string orderId)
        {
            MySagaManager.Handle<MyOrderWorkflow, PaymentReceived>(orderId, new PaymentReceived(orderId));

            AssertNoCommandProduced();
        }

        [Theory(DisplayName = "Ship the goods when the payment is received")]
        [AutoData]
        public void Test03(string orderId)
        {
            MySagaManager.Handle<MyOrderWorkflow, OrderCreated>(orderId, new OrderCreated(orderId));

            MySagaManager.Handle<MyOrderWorkflow, PaymentReceived>(orderId, new PaymentReceived(orderId));

            AssertCommandProduced(new ShipOrder(orderId));
        }

        #region Private

        private void AssertNoCommandProduced()
        {
            _outbox.Commands.Should().BeEmpty();
        }

        private void AssertCommandProduced<T>(T command) where T : ISagaCommand
        {
            _outbox.Commands.Should().Contain(command);
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

        public MyOrderWorkflow(string id)
            : base(id)
        {
        }

        public void Handle(OrderCreated @event)
        {
            Apply(@event);

            Publish(new CreateOrder(@event.OrderId));
        }

        public void Handle(PaymentReceived @event)
        {
            Apply(@event);

            if (_created)
            {
                Publish(new ShipOrder(@event.OrderId));
            }
        }

        public void Apply(OrderCreated @event)
        {
            _created = true;
        }

        public void Apply(PaymentReceived @event)
        {
        }
    }
}