using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Orchestrator
{
    public class Initiate
    {
        private readonly ILogger _logger;

        public Initiate(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Initiate>();
        }

        [Function("Initiate")]
        public void Run([ServiceBusTrigger("initiate", Connection = "SBConnectionString")] string myQueueItem)
        {
            _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
