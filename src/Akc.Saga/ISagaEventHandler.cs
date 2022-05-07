namespace Akc.Saga
{
    public interface ISagaEventHandler<T> where T : ISagaEvent
    {
        Task Handle(T @event, IMessageContext context);
    }
}
