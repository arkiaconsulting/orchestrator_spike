namespace Akc.Saga.InMemory
{
    internal class InMemorySagaCommandOutbox : ISagaCommandOutbox
    {
        public IEnumerable<ISagaCommand> Commands => _commands;

        private readonly ICollection<ISagaCommand> _commands = new List<ISagaCommand>();
        private readonly ISagaCommandPublisher messageBus;

        public InMemorySagaCommandOutbox(ISagaCommandPublisher messageBus)
        {
            this.messageBus = messageBus;
        }

        Task ISagaCommandOutbox.Publish<T>(T command)
        {
            return messageBus.Publish(command);
        }
    }
}
