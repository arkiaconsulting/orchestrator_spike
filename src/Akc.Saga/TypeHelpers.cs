using System.Reflection;

namespace Akc.Saga
{
    internal static class TypeHelpers
    {
        public static string GetEventTypeName(Type eventType)
        {
            return (eventType.GetCustomAttribute<EventTypeNameAttribute>()
                ?? throw new InvalidOperationException($"Cannot resolve event type name from {eventType.Name}")
            ).EventTypeName;
        }

        public static string GetCommandTypeName(Type eventType)
        {
            return (eventType.GetCustomAttribute<CommandTypeNameAttribute>()
                ?? throw new InvalidOperationException($"Cannot resolve command type name from {eventType.Name}")
            ).CommandTypeName;
        }
    }
}
