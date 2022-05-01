namespace Akc.Saga
{
    public interface ISagaMessageBus
    {
        Task Publish<T>(T message) where T : ISagaCommand;
    }
}