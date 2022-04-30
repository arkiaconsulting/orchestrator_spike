using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Orchestrator
{
    public class SecurityCheckedHandler
    {
        private readonly ILogger _logger;

        public SecurityCheckedHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SecurityCheckedHandler>();
        }

        [Function("SecurityCheckedHandler")]
        public void Run([ServiceBusTrigger("orchestrator", "security-checked", Connection = "SBConnectionString")] string myQueueItem)
        {
            _logger.LogInformation("Handling Dispatched");
        }
    }
}
