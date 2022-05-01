namespace Akc.Saga
{
    public class SagaManager
    {
        private readonly ISagaEventStore eventStore;
        private readonly ISagaOutbox outbox;
        private readonly IDictionary<Type, Func<string, Saga>> _sagas = new Dictionary<Type, Func<string, Saga>>();

        internal SagaManager(ISagaEventStore eventStore, ISagaOutbox outbox)
        {
            this.eventStore = eventStore;
            this.outbox = outbox;
        }

        public void Handle<T, TEvent>(string rootId, TEvent @event)
            where T : Saga, ISagaEventHandler<TEvent>
            where TEvent : ISagaEvent
        {
            var saga = Get<T>(rootId);

            saga.Handle(@event);

            foreach (var command in saga.PendingCommands)
            {
                outbox.Publish(command);
            }

            eventStore.Save(rootId, @event);
        }

        private T Get<T>(string rootId) where T : Saga
        {
            var instance = _sagas[typeof(T)](rootId);

            var events = eventStore.Load(rootId);
            instance.Rehydrate(events);

            return (T)instance;
        }

        internal void Register<T>(Func<string, T> instanciator) where T : Saga
        {
            _sagas.Add(typeof(T), instanciator);
        }
    }
}
