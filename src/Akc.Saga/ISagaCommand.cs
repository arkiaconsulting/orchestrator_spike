namespace Akc.Saga
{
    public interface ISagaCommand
    {
        string Type { get; }
    }
}