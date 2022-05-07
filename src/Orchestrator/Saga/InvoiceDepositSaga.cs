using Akc.Saga;
using ASaga = Akc.Saga.Saga;

namespace Orchestrator.Saga
{
    internal class InvoiceDepositSaga : ASaga,
        ISagaEventHandler<InitiatedSagaEvent>,
        ISagaEventHandler<SecurityCheckedSagaEvent>,
        ISagaEventHandler<DispatchedSagaEvent>
    {
        public void Handle(InitiatedSagaEvent @event, IMessageContext context)
        {
            Publish(new CheckSecuritySagaCommand(@event.TicketId), context);
        }

        public void Handle(SecurityCheckedSagaEvent @event, IMessageContext context)
        {
            Publish(new DispatchSagaCommand(@event.TicketId), context);
        }

        public void Handle(DispatchedSagaEvent @event, IMessageContext context)
        {
        }
    }
}
