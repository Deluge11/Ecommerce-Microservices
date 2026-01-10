using System.Security.Claims;

namespace Presentation_Layer.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger<ExceptionLoggingMiddleware> Logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unhandled exception | CorrelationId: {CorrelationId}", context.Items["CorrelationId"]);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Internal Server Error : {ex}");
            }
        }
    }

}
