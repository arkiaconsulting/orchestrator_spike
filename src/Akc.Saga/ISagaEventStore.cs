namespace Akc.Saga
{
    public interface ISagaEventStore
    {
        IEnumerable<ISagaEvent> Load(string rootId);
        void Save(string rootId, ISagaEvent @event);
    }
}
