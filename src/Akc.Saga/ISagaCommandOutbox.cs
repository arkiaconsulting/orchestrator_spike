namespace Akc.Saga
{
    public interface ISagaCommandOutbox
    {
        Task Publish<T>(T command) where T : ISagaCommand;
    }
}
