namespace Akc.Saga
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandTypeNameAttribute : Attribute
    {
        public string CommandTypeName { get; }

        public CommandTypeNameAttribute(string commandTypeName)
        {
            CommandTypeName = commandTypeName;
        }
    }
}
