using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EntryPoint
{
    internal static class ConfigurationExtensions
    {
        public static IServiceCollection AddInitiateSender(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSingleton(_ =>
                new InitiateSender(new ServiceBusClient(configuration["SBConnectionString"]))
            );
        }
    }

    public class InitiateSender : ServiceBusSender
    {
        public InitiateSender(ServiceBusClient serviceBusClient)
            : base(serviceBusClient, "initiate")
        {
        }
    }
}
