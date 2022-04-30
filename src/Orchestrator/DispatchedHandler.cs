using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Orchestrator
{
    public class DispatchedHandler
    {
        private readonly ILogger _logger;

        public DispatchedHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DispatchedHandler>();
        }

        [Function("DispatchedHandler")]
        public void Run([ServiceBusTrigger("orchestrator", "dispatched", Connection = "SBConnectionString")] string myQueueItem)
        {
            _logger.LogInformation("Handling Dispatched");
        }
    }
}
