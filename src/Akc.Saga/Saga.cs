using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Akc.Saga.Tests")]

namespace Akc.Saga
{
    public abstract class Saga
    {
        internal IEnumerable<ISagaCommand> PendingCommands => _pendingCommands;

        private readonly ICollection<ISagaCommand> _pendingCommands = new List<ISagaCommand>();

        protected void Publish<T>(T command, IMessageContext context) where T : ISagaCommand
        {
            if (!context.IsRehydrating)
            {
                _pendingCommands.Add(command);
            }
        }

        internal void Rehydrate(IEnumerable<ISagaEvent> events)
        {
            var messageContext = new AkcSagaMessageContext { IsRehydrating = true };

            foreach (var @event in events)
            {
                var handlerType = typeof(ISagaEventHandler<>).MakeGenericType(@event.GetType());
                var method = handlerType.GetMethod(nameof(ISagaEventHandler<ISagaEvent>.Handle));

                method!.Invoke(this, new object[] { @event, messageContext });
            }
        }
    }
}
