using Microsoft.Extensions.Hosting;
using Orchestrator;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
        services.AddSecurityCheckSender(ctx.Configuration).AddDispatchSender(ctx.Configuration)
    )
    .Build();

host.Run();