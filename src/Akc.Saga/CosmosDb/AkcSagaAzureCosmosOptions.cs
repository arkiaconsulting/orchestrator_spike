using System.ComponentModel.DataAnnotations;

namespace Microsoft.Extensions.DependencyInjection
{
    public class AkcSagaAzureCosmosOptions
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public string Database { get; set; } = string.Empty;

        [Required]
        public string OutboxContainer { get; set; } = string.Empty;

        public bool PreferMsi { get; set; } = true;
    }
}