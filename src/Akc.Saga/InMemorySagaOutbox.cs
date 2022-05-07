namespace Akc.Saga
{
    internal class InMemorySagaOutbox : ISagaOutbox
    {
        IEnumerable<ISagaCommand> ISagaOutbox.Commands => _commands;

        private readonly ICollection<ISagaCommand> _commands = new List<ISagaCommand>();
        private readonly ISagaMessageBus messageBus;

        public InMemorySagaOutbox(ISagaMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        void ISagaOutbox.Publish<T>(T command)
        {
            messageBus.Publish(command);
        }
    }
}
