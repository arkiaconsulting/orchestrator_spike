namespace Akc.Saga
{
    public class SagaHost
    {
        private readonly AkcSagaConfiguration configuration;
        private readonly ISagaEventStore eventStore;
        private readonly ISagaOutbox outbox;

        internal SagaHost(
            AkcSagaConfiguration configuration,
            ISagaEventStore eventStore,
            ISagaOutbox outbox)
        {
            this.configuration = configuration;
            this.eventStore = eventStore;
            this.outbox = outbox;
        }

        public void Handle<T, TEvent>(TEvent @event)
            where T : Saga, ISagaEventHandler<TEvent>, new()
            where TEvent : ISagaEvent
        {
            var sagaId = configuration.GetSagaId<T, TEvent>(@event);
            var saga = Get<T>(sagaId);

            saga.Handle(@event, new AkcSagaMessageContext { IsRehydrating = false });

            foreach (var command in saga.PendingCommands)
            {
                outbox.Publish(command);
            }

            eventStore.Save(sagaId, @event);
        }

        private T Get<T>(string rootId) where T : Saga, new()
        {
            var instance = new T();

            var events = eventStore.Load(rootId);
            instance.Rehydrate(events);

            return (T)instance;
        }
    }
}
