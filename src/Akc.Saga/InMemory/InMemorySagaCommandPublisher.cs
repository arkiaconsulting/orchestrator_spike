namespace Akc.Saga.InMemory
{
    internal class InMemorySagaCommandPublisher : ISagaCommandPublisher
    {
        public IEnumerable<ISagaCommand> Commands => _publishedCommands;

        private readonly ICollection<ISagaCommand> _publishedCommands = new List<ISagaCommand>();

        Task ISagaCommandPublisher.Publish<T>(T message)
        {
            _publishedCommands.Add(message);

            return Task.CompletedTask;
        }
    }
}
