namespace Akc.Saga.CosmosDb
{
    internal record CosmosDocumentEnveloppe(string Id, string TypeName, byte[] Payload);
}
