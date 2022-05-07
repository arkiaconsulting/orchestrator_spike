using Akc.Saga;
using Akc.Saga.AzureServiceBus;
using Akc.Saga.InMemory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddAkcSagaAzureServiceBus(
            this IServiceCollection services,
            Action<AkcSagaAzureServiceBusOptions> configure)
        {
            services.AddOptions<AkcSagaAzureServiceBusOptions>()
                .Configure(configure);

            services
                .AddSingleton<AzureServiceBusEntityLocator>()
                .AddSingleton<ISagaEventStore, InMemorySagaEventStore>()
                .AddTransient<ISagaCommandPublisher, AzureServiceBusCommandPublisher>();

            return services;
        }
    }
}
