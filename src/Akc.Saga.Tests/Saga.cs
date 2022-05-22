using System.Threading.Tasks;

namespace Akc.Saga.Tests
{
    [EventTypeName("OrderCreated")]
    internal record OrderCreated(string OrderId) : ISagaEvent
    {
    }

    [EventTypeName("PaymentReceived")]
    internal record PaymentReceived(string OrderId) : ISagaEvent
    {
    }

    [CommandTypeName("CreateOrder")]
    internal record CreateOrder(string OrderId) : ISagaCommand
    {
    }

    [CommandTypeName("ShipOrder")]
    internal record ShipOrder(string OrderId) : ISagaCommand
    {
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
                MarkAsComplete();

                return Publish(new ShipOrder(@event.OrderId), context);
            }

            return Task.CompletedTask;
        }
    }
}
