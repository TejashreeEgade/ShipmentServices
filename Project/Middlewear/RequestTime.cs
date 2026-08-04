using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Project.Middlewear
{
    public class RequestTime
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTime> _logger;
        public RequestTime(RequestDelegate next, ILogger<RequestTime> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
                                                     
         {
            var time = Stopwatch.StartNew();
            await _next(context);
            time.Stop();
            var elapsedMs = time.ElapsedMilliseconds;

            _logger.LogInformation("", context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsedMs);

            return;
        }
    }

}
