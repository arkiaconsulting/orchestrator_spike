using Akc.Saga;

namespace Orchestrator.Saga
{
    internal record CheckSecuritySagaCommand(Guid TicketId) : ISagaCommand
    {
        public string Type { get; } = "CheckSecurity";
    }
}
