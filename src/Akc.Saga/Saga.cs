using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Akc.Saga.Tests")]

namespace Akc.Saga
{
    public abstract class Saga
    {
        public bool Completed { get; private set; }

        internal IEnumerable<ISagaCommand> PendingCommands => _pendingCommands;

        private readonly ICollection<ISagaCommand> _pendingCommands = new List<ISagaCommand>();

        protected Task Publish<T>(T command, IMessageContext context) where T : ISagaCommand
        {
            if (!context.IsRehydrating)
            {
                _pendingCommands.Add(command);
            }

            return Task.CompletedTask;
        }

        protected void MarkAsComplete()
        {
            Completed = true;
        }

        internal async Task Rehydrate(IEnumerable<ISagaEvent> events)
        {
            var messageContext = new MessageContext { IsRehydrating = true };

            foreach (var @event in events)
            {
                var handlerType = typeof(ISagaEventHandler<>).MakeGenericType(@event.GetType());
                var method = handlerType.GetMethod(nameof(ISagaEventHandler<ISagaEvent>.Handle));

                var task = (Task)method!.Invoke(this, new object[] { @event, messageContext })!;
                await task;
            }
        }
    }
}
