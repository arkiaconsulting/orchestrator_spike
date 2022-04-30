using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BusinessWorker
{
    public class Dispatch
    {
        private readonly ILogger _logger;

        public Dispatch(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Dispatch>();
        }

        [Function("Dispatch")]
        public void Run([ServiceBusTrigger("dispatch", Connection = "SBConnectionString")] string myQueueItem)
        {
            _logger.LogInformation("Producing Dispatched");
        }
    }
}
