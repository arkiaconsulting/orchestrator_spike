using Akc.Saga;

namespace Orchestrator.Saga
{
    internal record InitiatedSagaEvent(Guid TicketId) : ISagaEvent;
}
