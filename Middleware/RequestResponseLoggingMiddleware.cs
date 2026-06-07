using System.Diagnostics;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            // Log incoming request
            var requestBody = await ReadRequestBodyAsync(request);
            _logger.LogInformation($"[REQUEST] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} | " +
                $"Method: {request.Method} | " +
                $"Path: {request.Path} | " +
                $"QueryString: {request.QueryString} | " +
                $"Body: {requestBody}");

            // Store original response stream
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();
                    
                    // Log outgoing response
                    var response = context.Response;
                    var responseContent = await ReadResponseBodyAsync(response);
                    
                    _logger.LogInformation($"[RESPONSE] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} | " +
                        $"StatusCode: {response.StatusCode} | " +
                        $"ContentType: {response.ContentType} | " +
                        $"Duration: {stopwatch.ElapsedMilliseconds}ms | " +
                        $"Body: {responseContent}");

                    // Copy response back to original stream
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                request.EnableBuffering();
                var body = await new StreamReader(request.Body).ReadToEndAsync();
                request.Body.Position = 0;
                return string.IsNullOrEmpty(body) ? "[No Body]" : body;
            }
            catch
            {
                return "[Unable to Read Body]";
            }
        }

        private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            try
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);
                return string.IsNullOrEmpty(body) ? "[No Body]" : body;
            }
            catch
            {
                return "[Unable to Read Body]";
            }
        }
    }
}