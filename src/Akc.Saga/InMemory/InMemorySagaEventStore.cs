namespace Akc.Saga.InMemory
{
    internal class InMemorySagaEventStore : ISagaEventStore
    {
        private readonly IDictionary<string, ICollection<ISagaEvent>> _events = new Dictionary<string, ICollection<ISagaEvent>>();

        Task<IEnumerable<ISagaEvent>> ISagaEventStore.Load(string rootId)
        {
            if (!_events.ContainsKey(rootId))
            {
                return Task.FromResult(Enumerable.Empty<ISagaEvent>());
            }

            return Task.FromResult<IEnumerable<ISagaEvent>>(_events[rootId]);
        }

        Task ISagaEventStore.Save<TEvent>(string sagaId, TEvent @event)
        {
            if (!_events.ContainsKey(sagaId))
            {
                _events.Add(sagaId, new List<ISagaEvent>());
            }

            _events[sagaId].Add(@event);

            return Task.CompletedTask;
        }
    }
}
