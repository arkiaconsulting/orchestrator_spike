
namespace Akc.Saga
{
    internal class AkcSagaMessageContext : IMessageContext
    {
        public bool IsRehydrating { get; internal set; }
    }
}
