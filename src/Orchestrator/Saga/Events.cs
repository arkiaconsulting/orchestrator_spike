using Akc.Saga;

namespace Orchestrator.Saga
{
    internal record InitiatedSagaEvent(string TicketId) : ISagaEvent;
    internal record SecurityCheckedSagaEvent(string TicketId) : ISagaEvent;
    internal record DispatchedSagaEvent(string TicketId) : ISagaEvent;
}
