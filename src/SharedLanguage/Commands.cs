namespace SharedLanguage
{
    public record InitiateCommand(Guid TicketId);
    public record CheckSecurityCommand(Guid TicketId);
    public record DispatchCommand(Guid TicketId);
}