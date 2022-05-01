using Microsoft.Extensions.DependencyInjection;

namespace Akc.Saga
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAkcSagaAzureServiceBus(
            this IServiceCollection services,
            Action<AkcSagaAzureServiceBusOptions> configure)
        {
            services.AddOptions<AkcSagaAzureServiceBusOptions>()
                .Configure(configure);

            services.AddSingleton<AzureServiceBusEntityLocator>();
            services.AddTransient<ISagaMessageBus, AzureServiceBusMessageBus>();

            return services;
        }
    }
}
