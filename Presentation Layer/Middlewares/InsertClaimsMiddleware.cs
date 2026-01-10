namespace Presentation_Layer.Middlewares
{
    using System.Security.Claims;

    public class InsertClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public InsertClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.Request.Headers["X-User-Id"].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };

                var identity = new ClaimsIdentity(claims, "NginxAuth");
                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }
}
