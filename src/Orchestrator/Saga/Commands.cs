using Akc.Saga;

namespace Orchestrator.Saga
{
    [CommandTypeName("CheckSecurity")]
    internal record CheckSecuritySagaCommand(string TicketId) : ISagaCommand;

    [CommandTypeName("Dispatch")]
    internal record DispatchSagaCommand(string TicketId) : ISagaCommand;
}
