using System.Net;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SharedLanguage;

namespace EntryPoint
{
    public class Go
    {
        private readonly ILogger<Go> logger;
        private readonly InitiateSender sender;

        public Go(
            ILogger<Go> logger,
            InitiateSender sender)
        {
            this.logger = logger;
            this.sender = sender;
        }

        [Function("Go")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext context)
        {
            var ticketId = Guid.NewGuid();
            logger.LogInformation("Initiating process {TicketId}", ticketId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString($"TicketId: {ticketId}");

            logger.LogInformation($"Publishing {nameof(InitiateCommand)} {{TicketId}}", ticketId);

            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(new InitiateCommand(ticketId)))
            {
                CorrelationId = context.TraceContext.TraceParent
            };
            await sender.SendMessageAsync(message);

            return response;
        }
    }

    //public class Output
    //{
    //    public HttpResponseData HttpResponse { get; set; }

    //    [ServiceBusOutput("initiate", Connection = "SBConnectionString")]
    //    public IEnumerable<InitiateCommand> ServiceBusMessages { get; set; }

    //    public Output(HttpResponseData httpResponse, IEnumerable<InitiateCommand> serviceBusMessages)
    //    {
    //        HttpResponse = httpResponse;
    //        ServiceBusMessages = serviceBusMessages;
    //    }
    //}
}
