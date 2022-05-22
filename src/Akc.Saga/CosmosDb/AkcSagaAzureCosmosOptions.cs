using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Akc.Saga.CosmosDb
{
    public class AkcSagaAzureCosmosOptions
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public string Database { get; set; } = string.Empty;

        public string OutboxContainer { get; set; } = string.Empty;

        public string EventStoreContainer { get; set; } = string.Empty;

        public bool PreferMsi { get; set; } = true;

        internal JsonSerializerOptions PayloadSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
}