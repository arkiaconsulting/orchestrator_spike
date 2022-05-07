using Akc.Saga;
using Akc.Saga.InMemory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ConfigurationExtensions
    {
        public static IServiceCollection AddAkcSaga(
            this IServiceCollection services,
            Action<AkcSagaConfiguration> configure
            )
        {
            var sagaConfiguration = new AkcSagaConfiguration();
            configure(sagaConfiguration);

            return services
                .AddSingleton<ISagaEventStore, InMemorySagaEventStore>()
                .AddSingleton<ISagaCommandOutbox, InMemorySagaCommandOutbox>()
                .AddSingleton<ISagaCommandPublisher, InMemorySagaCommandPublisher>()
                .AddSingleton(sagaConfiguration)
                .AddTransient(sp =>
                {
                    return new SagaHost(
                        sagaConfiguration,
                        sp.GetRequiredService<ISagaEventStore>(),
                        sp.GetRequiredService<ISagaCommandOutbox>()
                    );
                });
        }
    }
}
