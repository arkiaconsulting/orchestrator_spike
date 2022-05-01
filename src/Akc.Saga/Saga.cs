using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Akc.Saga.Tests")]

namespace Akc.Saga
{
    public abstract class Saga
    {
        internal IEnumerable<ISagaCommand> PendingCommands => _pendingCommands;

        protected readonly string id;
        private readonly ICollection<ISagaCommand> _pendingCommands = new List<ISagaCommand>();

        protected Saga(string id)
        {
            this.id = id;
        }

        protected void Publish<T>(T command) where T : ISagaCommand
        {
            _pendingCommands.Add(command);
        }

        internal void Rehydrate(IEnumerable<ISagaEvent> events)
        {
            foreach (var @event in events)
            {
                var method = this.GetType().GetRuntimeMethods()
                    .Single(m => m.Name == "Apply" && m.GetParameters().SingleOrDefault(p => p.ParameterType == @event.GetType()) != null);

                method!.Invoke(this, new object[] { @event });
            }
        }
    }
}
