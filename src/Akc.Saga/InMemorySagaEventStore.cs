namespace Akc.Saga
{
    internal class InMemorySagaEventStore : ISagaEventStore
    {
        private readonly IDictionary<string, ICollection<ISagaEvent>> _events = new Dictionary<string, ICollection<ISagaEvent>>();

        IEnumerable<ISagaEvent> ISagaEventStore.Load(string rootId)
        {
            if (!_events.ContainsKey(rootId))
            {
                return Enumerable.Empty<ISagaEvent>();
            }

            return _events[rootId];
        }

        void ISagaEventStore.Save(string rootId, ISagaEvent @event)
        {
            if (!_events.ContainsKey(rootId))
            {
                _events.Add(rootId, new List<ISagaEvent>());
            }

            _events[rootId].Add(@event);
        }
    }
}
