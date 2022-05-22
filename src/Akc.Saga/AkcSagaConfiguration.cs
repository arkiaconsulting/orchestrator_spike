using System.Reflection;

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

        internal IDictionary<string, Type> NameToEventType { get; private set; } = new Dictionary<string, Type>();
        internal IDictionary<Type, string> EventTypeToName { get; private set; } = new Dictionary<Type, string>();
        internal IDictionary<string, Type> NameToCommandType { get; private set; } = new Dictionary<string, Type>();
        internal IDictionary<Type, string> CommandTypeToName { get; private set; } = new Dictionary<Type, string>();

        public void RegisterEvents(Assembly assembly)
        {
            NameToEventType = assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ISagaEvent)))
                .ToDictionary(type => TypeHelpers.GetEventTypeName(type), type => type);

            EventTypeToName = NameToEventType
                .ToDictionary(kv => kv.Value, kv => kv.Key);
        }

        public void RegisterCommands(Assembly assembly)
        {
            NameToCommandType = assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ISagaCommand)))
                .ToDictionary(type => TypeHelpers.GetCommandTypeName(type), type => type);

            CommandTypeToName = NameToCommandType
                .ToDictionary(kv => kv.Value, kv => kv.Key);
        }
    }
}