namespace Akc.Saga
{
    public interface ISagaOutbox
    {
        IEnumerable<ISagaCommand> Commands { get; }

        void Publish<T>(T command) where T : ISagaCommand;
    }
}
