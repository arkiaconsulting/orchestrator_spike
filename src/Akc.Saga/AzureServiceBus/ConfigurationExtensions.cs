using Akc.Saga;
using Akc.Saga.AzureServiceBus;

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
                .AddTransient<ISagaCommandPublisher, AzureServiceBusCommandPublisher>();

            return services;
        }
    }
}
