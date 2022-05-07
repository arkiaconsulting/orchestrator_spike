namespace Akc.Saga
{
    public class AkcSagaConfiguration
    {
        private readonly IDictionary<Type, IDictionary<Type, Func<ISagaEvent, string>>> _sagaIdProviders =
            new Dictionary<Type, IDictionary<Type, Func<ISagaEvent, string>>>();

        internal string GetSagaId<TSaga, TEvent>(TEvent @event) where TEvent : ISagaEvent
        {
            var sagaIdProvider = _sagaIdProviders[typeof(TSaga)][typeof(TEvent)];

            return sagaIdProvider(@event);
        }

        public void Register<TSaga, TEvent>(Func<TEvent, string> value)
            where TSaga : Saga
            where TEvent : ISagaEvent
        {
            var sagaType = typeof(TSaga);
            var eventType = typeof(TEvent);

            if (!_sagaIdProviders.ContainsKey(sagaType))
            {
                _sagaIdProviders.Add(sagaType, new Dictionary<Type, Func<ISagaEvent, string>>());
            }

            _sagaIdProviders[sagaType].Add(eventType, e => value((TEvent)e));
        }
    }
}