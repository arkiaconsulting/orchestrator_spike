using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orchestrator;
using Orchestrator.Saga;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
        services
        .AddSecurityCheckSender(ctx.Configuration)
        .AddDispatchSender(ctx.Configuration)
        .AddAkcSaga(config =>
        {
            config.Register<InvoiceDepositSaga, InitiatedSagaEvent>(e => e.TicketId);
            config.Register<InvoiceDepositSaga, SecurityCheckedSagaEvent>(e => e.TicketId);
            config.Register<InvoiceDepositSaga, DispatchedSagaEvent>(e => e.TicketId);
            config.RegisterEvents(typeof(InitiatedSagaEvent).Assembly);
            config.RegisterCommands(typeof(CheckSecuritySagaCommand).Assembly);
        })
        .AddAkcSagaAzureServiceBus(options =>
        {
            options.RegisterMessageEntity<CheckSecuritySagaCommand>("security-check");
            options.RegisterMessageEntity<DispatchSagaCommand>("dispatch");
        })
        .AddAkcSagaAzureCosmosDb(ctx.Configuration)
        .AddAkcSagaAzureCosmosEventStore()
        .AddAkcSagaAzureCosmosOutbox()
    )
    .Build();

host.Run();