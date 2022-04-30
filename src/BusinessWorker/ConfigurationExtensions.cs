using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessWorker
{
    internal static class ConfigurationExtensions
    {
        public static IServiceCollection AddOrchestratorSender(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSingleton(_ =>
                new OrchestratorSender(new ServiceBusClient(configuration["SBConnectionString"]))
            );
        }
    }

    public class OrchestratorSender : ServiceBusSender
    {
        public OrchestratorSender(ServiceBusClient serviceBusClient)
            : base(serviceBusClient, "orchestrator")
        {
        }
    }
}
