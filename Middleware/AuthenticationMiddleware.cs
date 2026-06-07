namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for health check and public endpoints
            if (context.Request.Path.StartsWithSegments("/api/auth/login") ||
                context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            var token = ExtractTokenFromHeader(context);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Missing authentication token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Missing authentication token" });
                return;
            }

            await _next(context);
        }

        private static string ExtractTokenFromHeader(HttpContext context)
        {
            const string scheme = "Bearer ";
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith(scheme))
            {
                return authHeader.Substring(scheme.Length).Trim();
            }

            return null;
        }
    }
}