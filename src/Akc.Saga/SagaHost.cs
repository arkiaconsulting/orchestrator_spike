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

        public async Task Handle<T, TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where T : Saga, ISagaEventHandler<TEvent>, new()
            where TEvent : ISagaEvent
        {
            var sagaId = configuration.GetSagaId<T, TEvent>(@event);
            var saga = await Get<T>(sagaId, cancellationToken).ConfigureAwait(false);

            if (saga.Completed)
            {
                return;
            }

            await saga.Handle(@event, new MessageContext { IsRehydrating = false }).ConfigureAwait(false);

            foreach (var command in saga.PendingCommands)
            {
                await outbox.Publish(command).ConfigureAwait(false);
            }

            await eventStore.Save(sagaId, @event, cancellationToken).ConfigureAwait(false);
        }

        private async Task<T> Get<T>(string rootId, CancellationToken cancellationToken = default) where T : Saga, new()
        {
            var instance = new T();

            var events = await eventStore.Load(rootId, cancellationToken).ConfigureAwait(false);
            await instance.Rehydrate(events).ConfigureAwait(false);

            return instance;
        }
    }
}
