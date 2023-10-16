using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;
using System.Text;

namespace RocketPDF.Infrastructure.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger Logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sw = Stopwatch.StartNew();

            context.Request.EnableBuffering();

            await Next(context);

            sw.Stop();

            await LogRequest(context, sw);
        }

        private async Task LogRequest(HttpContext context, Stopwatch sw, Exception? ex = default, Guid? errorId = null)
        {
            // Push the user name into the log context so that it is included in all log entries
            using (LogContext.PushProperty("CorrelationId", context.GetCorrelationId()))
            using (LogContext.PushProperty("RequestIp", context.Connection.RemoteIpAddress?.ToString()))
            using (LogContext.PushProperty("RequestTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))
            using (LogContext.PushProperty("UserName", context.User.Identity?.Name ?? "Unauthorized"))
            using (LogContext.PushProperty("StatusCode", context.Response.StatusCode))
            using (LogContext.PushProperty("ErrorId", errorId?.ToString() ?? string.Empty))
            {
                var requestBody = string.Empty;
                if ((HttpMethods.IsPost(context.Request.Method) || HttpMethods.IsPut(context.Request.Method) || HttpMethods.IsPatch(context.Request.Method)) && context.Request.ContentType == "application/json")
                {
                    requestBody = await ReadResponseBody(context.Request);
                }
                var log = $"HTTP \"{context.Request.Method}\" \"{context.Request.Host}{context.Request.Path}{context.Request.QueryString}\" responded {context.Response.StatusCode} in {sw.ElapsedMilliseconds:0.0000} ms {requestBody}".Trim();
                if (context.Response.StatusCode >= StatusCodes.Status200OK && context.Response.StatusCode <= StatusCodes.Status308PermanentRedirect)
                    Logger.LogInformation(log);
                else if (context.Response.StatusCode >= StatusCodes.Status400BadRequest && context.Response.StatusCode <= StatusCodes.Status451UnavailableForLegalReasons)
                    Logger.LogWarning(log);
                else if (context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                    Logger.LogError(ex, log);
            }
        }

        private async Task<string> ReadResponseBody(HttpRequest request)
        {
            try
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
                string requestBody = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);
                return requestBody;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Read request body error");
                return string.Empty;
            }
        }
    }

    public static class RequestLoggingExtension
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}