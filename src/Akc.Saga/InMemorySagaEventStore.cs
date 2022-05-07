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

        void ISagaEventStore.Save<TEvent>(string sagaId, TEvent @event)
        {
            if (!_events.ContainsKey(sagaId))
            {
                _events.Add(sagaId, new List<ISagaEvent>());
            }

            _events[sagaId].Add(@event);
        }
    }
}
