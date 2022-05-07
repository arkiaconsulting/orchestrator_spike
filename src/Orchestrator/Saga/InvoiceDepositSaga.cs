using Akc.Saga;
using ASaga = Akc.Saga.Saga;

namespace Orchestrator.Saga
{
    internal class InvoiceDepositSaga : ASaga,
        ISagaEventHandler<InitiatedSagaEvent>,
        ISagaEventHandler<SecurityCheckedSagaEvent>,
        ISagaEventHandler<DispatchedSagaEvent>
    {
        public Task Handle(InitiatedSagaEvent @event, IMessageContext context)
        {
            return Publish(new CheckSecuritySagaCommand(@event.TicketId), context);
        }

        public Task Handle(SecurityCheckedSagaEvent @event, IMessageContext context)
        {
            return Publish(new DispatchSagaCommand(@event.TicketId), context);
        }

        public Task Handle(DispatchedSagaEvent @event, IMessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}
