using System.Threading.Tasks;

namespace Akc.Saga.Tests
{
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

        public Task Handle(OrderCreated @event, IMessageContext context)
        {
            _created = true;

            return Publish(new CreateOrder(@event.OrderId), context);
        }

        public Task Handle(PaymentReceived @event, IMessageContext context)
        {
            if (_created)
            {
                return Publish(new ShipOrder(@event.OrderId), context);
            }

            return Task.CompletedTask;
        }
    }
}
