namespace Akc.Saga
{
    internal class InMemorySagaMessageBus : ISagaMessageBus
    {
        IEnumerable<ISagaCommand> ISagaMessageBus.Commands => _publishedCommands;

        private readonly ICollection<ISagaCommand> _publishedCommands = new List<ISagaCommand>();

        Task ISagaMessageBus.Publish<T>(T message)
        {
            _publishedCommands.Add(message);

            return Task.CompletedTask;
        }
    }
}
