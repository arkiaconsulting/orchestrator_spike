namespace Akc.Saga.CosmosDb
{
    internal record CosmosSagaEvent(string Id, string SagaId, string Type, string Payload, DateTimeOffset CreatedAt);
}
