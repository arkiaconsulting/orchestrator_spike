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

            services
                .AddSingleton<AzureServiceBusEntityLocator>()
                .AddSingleton<ISagaEventStore, InMemorySagaEventStore>()
                .AddTransient<ISagaMessageBus, AzureServiceBusMessageBus>();

            return services;
        }

        public static IServiceCollection AddAkcSaga(
            this IServiceCollection services,
            Action<AkcSagaConfiguration> configure
            )
        {
            var sagaConfiguration = new AkcSagaConfiguration();
            configure(sagaConfiguration);

            return services
                .AddSingleton<ISagaEventStore, InMemorySagaEventStore>()
                .AddSingleton<ISagaOutbox, InMemorySagaOutbox>()
                .AddSingleton<ISagaMessageBus, InMemorySagaMessageBus>()
                .AddSingleton(sagaConfiguration)
                .AddTransient(sp =>
                {
                    return new SagaHost(
                        sagaConfiguration,
                        sp.GetRequiredService<ISagaEventStore>(),
                        sp.GetRequiredService<ISagaOutbox>()
                    );
                });
        }
    }
}
