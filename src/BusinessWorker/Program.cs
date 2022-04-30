using BusinessWorker;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) => services.AddOrchestratorSender(ctx.Configuration))
    .Build();

host.Run();