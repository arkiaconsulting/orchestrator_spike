namespace Akc.Saga
{
    public interface ISagaMessageBus
    {
        IEnumerable<ISagaCommand> Commands { get; }

        Task Publish<T>(T message) where T : ISagaCommand;
    }
}