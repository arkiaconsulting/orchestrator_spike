namespace Akc.Saga
{
    public interface ISagaEventStore
    {
        IEnumerable<ISagaEvent> Load(string sagaId);
        void Save<TEvent>(string sagaId, TEvent @event) where TEvent : ISagaEvent;
    }
}
