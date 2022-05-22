using Akc.Saga;
using Akc.Saga.CosmosDb;
using Azure.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddAkcSagaAzureCosmosDb(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<AkcSagaAzureCosmosOptions>? configure = default,
            Action<CosmosClientBuilder>? configureCosmosBuilder = default)
        {
            services.AddOptions<AkcSagaAzureCosmosOptions>()
                .Configure(o => { configuration.Bind("Akc:Cosmos", o); (configure ??= (o => { }))(o); })
                .ValidateDataAnnotations();

            services
                .AddSingleton(sp => BuildCosmosClient(sp, configureCosmosBuilder));

            return services;
        }

        public static IServiceCollection AddAkcSagaAzureCosmosOutbox(this IServiceCollection services)
        {
            services
                .AddSingleton<OutboxContainer>()
                .AddTransient<ISagaCommandOutbox, AzureCosmosDbCommandOutbox>()
                .AddHostedService<AzureCosmosOutboxListener>();

            return services;
        }

        public static IServiceCollection AddAkcSagaAzureCosmosEventStore(this IServiceCollection services)
        {
            services
                .AddSingleton<EventStoreContainer>()
                .AddTransient<ISagaEventStore, AzureCosmosDbEventStore>();

            return services;
        }

        private static CosmosClient BuildCosmosClient(IServiceProvider sp, Action<CosmosClientBuilder>? configureCosmosBuilder)
        {
            var options = sp.GetRequiredService<IOptions<AkcSagaAzureCosmosOptions>>().Value;

            CosmosClientBuilder? builder;
            if (options.PreferMsi)
            {
                builder = new CosmosClientBuilder(options.ConnectionString, sp.GetRequiredService<TokenCredential>());
            }
            else
            {
                builder = new CosmosClientBuilder(options.ConnectionString);
            }
            (configureCosmosBuilder ??= (b => { }))(builder);

            return builder
                .WithContentResponseOnWrite(false)
                .WithSerializerOptions(new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .WithConnectionModeGateway()
                .WithLimitToEndpoint(true)
                .Build();
        }
    }
}
