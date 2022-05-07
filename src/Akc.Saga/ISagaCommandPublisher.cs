namespace Akc.Saga
{
    public interface ISagaCommandPublisher
    {
        Task Publish<T>(T message) where T : ISagaCommand;
    }
}