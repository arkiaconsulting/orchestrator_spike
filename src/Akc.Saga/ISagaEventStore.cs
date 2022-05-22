namespace Akc.Saga
{
    public interface ISagaEventStore
    {
        Task<IEnumerable<ISagaEvent>> Load(string sagaId, CancellationToken cancellationToken = default);
        Task Save<TEvent>(string sagaId, TEvent @event, CancellationToken cancellationToken = default) where TEvent : ISagaEvent;
    }
}
