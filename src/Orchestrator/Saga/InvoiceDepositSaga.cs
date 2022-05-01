using Akc.Saga;
using ASaga = Akc.Saga.Saga;

namespace Orchestrator.Saga
{
    internal class InvoiceDepositSaga : ASaga,
        ISagaEventHandler<InitiatedSagaEvent>
    {
        public InvoiceDepositSaga(string id)
            : base(id)
        {
        }

        public void Handle(InitiatedSagaEvent @event)
        {
            Publish(new CheckSecuritySagaCommand(@event.TicketId));
        }

        public void Apply(InitiatedSagaEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
