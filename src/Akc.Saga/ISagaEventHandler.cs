namespace Akc.Saga
{
    public interface ISagaEventHandler<T> where T : ISagaEvent
    {
        void Handle(T @event);
        void Apply(T @event);
    }
}
