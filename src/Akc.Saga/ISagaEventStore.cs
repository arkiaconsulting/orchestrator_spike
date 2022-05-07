namespace Akc.Saga
{
    public interface ISagaEventStore
    {
        Task<IEnumerable<ISagaEvent>> Load(string sagaId);
        Task Save<TEvent>(string sagaId, TEvent @event) where TEvent : ISagaEvent;
    }
}
