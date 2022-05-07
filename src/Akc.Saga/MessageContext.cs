
namespace Akc.Saga
{
    internal class MessageContext : IMessageContext
    {
        public bool IsRehydrating { get; internal set; }
    }
}
