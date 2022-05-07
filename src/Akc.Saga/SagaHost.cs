namespace Akc.Saga
{
    public class SagaHost
    {
        private readonly AkcSagaConfiguration configuration;
        private readonly ISagaEventStore eventStore;
        private readonly ISagaCommandOutbox outbox;

        internal SagaHost(
            AkcSagaConfiguration configuration,
            ISagaEventStore eventStore,
            ISagaCommandOutbox outbox)
        {
            this.configuration = configuration;
            this.eventStore = eventStore;
            this.outbox = outbox;
        }

        public async Task Handle<T, TEvent>(TEvent @event)
            where T : Saga, ISagaEventHandler<TEvent>, new()
            where TEvent : ISagaEvent
        {
            var sagaId = configuration.GetSagaId<T, TEvent>(@event);
            var saga = await Get<T>(sagaId);

            await saga.Handle(@event, new MessageContext { IsRehydrating = false });

            foreach (var command in saga.PendingCommands)
            {
                await outbox.Publish(command);
            }

            await eventStore.Save(sagaId, @event);
        }

        private async Task<T> Get<T>(string rootId) where T : Saga, new()
        {
            var instance = new T();

            var events = await eventStore.Load(rootId);
            await instance.Rehydrate(events);

            return (T)instance;
        }
    }
}
