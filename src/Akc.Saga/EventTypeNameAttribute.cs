namespace Akc.Saga
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventTypeNameAttribute : Attribute
    {
        public string EventTypeName { get; }

        public EventTypeNameAttribute(string eventTypeName)
        {
            EventTypeName = eventTypeName;
        }
    }
}
