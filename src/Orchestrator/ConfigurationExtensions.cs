using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orchestrator
{
    internal static class ConfigurationExtensions
    {
        public static IServiceCollection AddDispatchSender(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSingleton(_ =>
                new DispatchSender(new ServiceBusClient(configuration["SBConnectionString"]))
            );
        }

        public static IServiceCollection AddSecurityCheckSender(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSingleton(_ =>
                new SecurityCheckSender(new ServiceBusClient(configuration["SBConnectionString"]))
            );
        }
    }

    public class DispatchSender : ServiceBusSender
    {
        public DispatchSender(ServiceBusClient serviceBusClient)
            : base(serviceBusClient, "dispatch")
        {
        }
    }

    public class SecurityCheckSender : ServiceBusSender
    {
        public SecurityCheckSender(ServiceBusClient serviceBusClient)
            : base(serviceBusClient, "security-check")
        {
        }
    }
}
