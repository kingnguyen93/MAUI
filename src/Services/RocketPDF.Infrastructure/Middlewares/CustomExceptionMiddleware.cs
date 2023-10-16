using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RocketPDF.Infrastructure.Exceptions;
using RocketPDF.Infrastructure.Models;
using Serilog.Context;
using System.Diagnostics;
using System.Text;

namespace RocketPDF.Infrastructure.Middlewares
{
    /// <summary>
    /// Middleware to handle exceptions.
    /// It separates exceptions based on their type and returns different status codes and answers based on it, instead of 500 Internal Server Error code in all cases.
    /// In addition, it writes them in the log.
    /// </summary>
    /// <remarks>
    /// There is another way to do this - an exception filter.
    /// However, a middleware is a preferred way to achieve this according to the official documentation.
    /// To learn more see https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.1#exception-filters
    /// </remarks>
    public class CustomExceptionMiddleware
    {
        private const int LongRequestTime = 5000;

        private RequestDelegate Next { get; }
        private ILogger Logger { get; }

        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sw = Stopwatch.StartNew();

            context.Request.EnableBuffering();

            var body = context.Response.Body;
            try
            {
                await Next(context);

                sw.Stop();

                if (sw.Elapsed.TotalMilliseconds >= LongRequestTime)
                {
                    await LogRequest(context, sw);
                }
            }
            catch (Exception ex)
            {
                sw.Stop();

                // If context.Response.HasStarted == true, then we can't write to the response stream anymore. So we have to restore the body.
                // If we don't do that we get an exception.
                context.Response.Body = body;

                await HandleExceptionAsync(context, ex, sw);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, Stopwatch sw)
        {
            context.Response.ContentType = "application/json";

            // We can decide what the status code should return
            if (ex is BadRequestException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(JsonHelper.Serialize(ApiResponseWrapper.BadRequest(message: ex.Message)));
            }
            else if (ex is NotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(JsonHelper.Serialize(ApiResponseWrapper.NotFound(message: ex.Message)));
            }
            else if (ex is ConflictException)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsync(JsonHelper.Serialize(ApiResponseWrapper.Conflict(message: ex.Message)));
            }
            else
            {
                var errorId = Guid.NewGuid();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
#if DEBUG
                await context.Response.WriteAsync(JsonHelper.Serialize(ApiResponseWrapper.InternalServerError(ex.ToString(), errorId, ex.GetBaseException().Message)));
#else
                await context.Response.WriteAsync(JsonHelper.Serialize(ApiResponseWrapper.InternalServerError(default, errorId, "Internal server error.")));
#endif

                await LogRequest(context, sw, ex, errorId);
            }
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

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomeExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>();
        }

        public static string GetCorrelationId(this HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("X-Correlation-Id", out StringValues correlationId);
            return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
        }
    }
}