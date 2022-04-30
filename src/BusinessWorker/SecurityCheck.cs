using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BusinessWorker
{
    public class SecurityCheck
    {
        private readonly ILogger _logger;

        public SecurityCheck(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SecurityCheck>();
        }

        [Function("SecurityCheck")]
        public void Run([ServiceBusTrigger("security-check", Connection = "SBConnectionString")] string myQueueItem)
        {
            _logger.LogInformation("Producing SecurityChecked");
        }
    }
}
