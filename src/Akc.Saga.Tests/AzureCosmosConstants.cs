using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akc.Saga.Tests
{
    internal static class AzureCosmosConstants
    {
        public const string ConnectionString = "AccountEndpoint=https://localhost:8081;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public const string Database = "orch-spike";
        public const string OutboxContainer = "tests-outbox";
        public const string EventStoreContainer = "tests-es";

        public static IServiceCollection AddTestCosmos(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddAkcSagaAzureCosmosDb(configuration,
                config =>
                {
                    config.ConnectionString = ConnectionString;
                    config.Database = Database;
                    config.OutboxContainer = OutboxContainer;
                    config.EventStoreContainer = EventStoreContainer;
                    config.PreferMsi = false;
                    config.RegisterEvents(Assembly.GetExecutingAssembly());
                }, builder => builder.WithConnectionModeGateway().WithLimitToEndpoint(true));
        }
    }
}
