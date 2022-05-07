
namespace Akc.Saga
{
    public interface IMessageContext
    {
        bool IsRehydrating { get; }
    }
}
