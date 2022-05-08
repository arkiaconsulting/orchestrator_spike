using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Akc.Saga;

namespace Microsoft.Extensions.DependencyInjection
{
    public class AkcSagaAzureCosmosOptions
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public string Database { get; set; } = string.Empty;

        public string OutboxContainer { get; set; } = string.Empty;

        public string EventStoreContainer { get; set; } = string.Empty;

        public bool PreferMsi { get; set; } = true;

        internal IDictionary<string, Type> NameToEventType { get; private set; } = new Dictionary<string, Type>();
        internal IDictionary<Type, string> EventTypeToName { get; private set; } = new Dictionary<Type, string>();

        public void RegisterEvents(Assembly assembly)
        {
            NameToEventType = assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ISagaEvent)))
                .ToDictionary(type => TypeHelpers.GetEventTypeName(type), type => type);

            EventTypeToName = NameToEventType
                .ToDictionary(kv => kv.Value, kv => kv.Key);
        }
    }
}