using Akc.Saga;

namespace Orchestrator.Saga
{
    [EventTypeName("Initiated")]
    internal record InitiatedSagaEvent(string TicketId) : ISagaEvent;

    [EventTypeName("SecurityChecked")]
    internal record SecurityCheckedSagaEvent(string TicketId) : ISagaEvent;

    [EventTypeName("Dispatched")]
    internal record DispatchedSagaEvent(string TicketId) : ISagaEvent;
}
