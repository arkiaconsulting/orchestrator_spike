namespace SharedLanguage
{
    public record SecurityCheckedEvent(Guid TicketId);
    public record DispatchedEvent(Guid TicketId);
}
