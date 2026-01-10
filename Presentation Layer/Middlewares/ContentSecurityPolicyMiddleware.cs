namespace Presentation_Layer.Middlewares;

public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;

    public ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
        await _next(context);
    }
}
