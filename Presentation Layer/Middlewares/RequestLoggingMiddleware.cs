namespace Presentation_Layer.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger<RequestLoggingMiddleware> Logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            Logger.LogInformation("Request {Method} {Path} | CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            await Next(context);

            Logger.LogInformation("Response {StatusCode} | CorrelationId: {CorrelationId}",
                context.Response.StatusCode,
                correlationId);
        }
    }



}
