namespace Akc.Saga
{
    internal class InMemorySagaOutbox : ISagaOutbox
    {
        IEnumerable<ISagaCommand> ISagaOutbox.Commands => _commands;

        private readonly ICollection<ISagaCommand> _commands = new List<ISagaCommand>();

        void ISagaOutbox.Publish<T>(T command)
        {
            _commands.Add(command);
        }
    }
}
