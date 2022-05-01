using Akc.Saga;
using Microsoft.Extensions.Hosting;
using Orchestrator;
using Orchestrator.Saga;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
        services
        .AddSecurityCheckSender(ctx.Configuration)
        .AddDispatchSender(ctx.Configuration)
        .AddAkcSagaAzureServiceBus(options =>
        {
            options.RegisterMessageEntity<CheckSecuritySagaCommand>("security-check");
        })
    )
    .Build();

host.Run();