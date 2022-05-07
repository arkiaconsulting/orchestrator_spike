using Akc.Saga;

namespace Orchestrator.Saga
{
    internal record CheckSecuritySagaCommand(string TicketId) : ISagaCommand
    {
        public string Type { get; } = "CheckSecurity";
    }

    internal record DispatchSagaCommand(string TicketId) : ISagaCommand
    {
        public string Type { get; } = "Dispatch";
    }
}
