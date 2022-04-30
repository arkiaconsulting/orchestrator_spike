using EntryPoint;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) => services.AddInitiateSender(ctx.Configuration))
    .Build();

host.Run();