using System.Diagnostics;
using System.Text;
using System.Text.Json;
using TestAppDev.Interfaces;
using TestAppDev.Models;

namespace TestAppDev.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private const int MaxBodySize = 8192;

    public ExceptionHandlingMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IExceptionJournalService journalService)
    {
        try
        {
            context.Request.EnableBuffering();
            await _next(context);
        }
        catch (Exception ex)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await HandleExceptionAsync(context, ex, journalService);
            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, IExceptionJournalService journalService)
    {
        var request = context.Request;
        var eventId = Guid.NewGuid().ToString();

        string bodyParameters = await ReadRequestBodySafeAsync(request);

        var exceptionJournal = new ExceptionJournal
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Timestamp = DateTime.UtcNow,
            ExceptionType = ex.GetType().Name,
            QueryParameters = request.QueryString.HasValue ? request.QueryString.Value : null,
            BodyParameters = bodyParameters,
            StackTrace = ex.StackTrace,
            ExceptionMessage = ex.Message,
        };

        await journalService.SaveExceptionToJournalAsync(exceptionJournal);

        context.Response.StatusCode = ex is SecureException
            ? StatusCodes.Status403Forbidden
            : StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        var response = new
        {
            type = ex.GetType().Name,
            id = eventId,
            data = new { message = ex.Message }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static async Task<string> ReadRequestBodySafeAsync(HttpRequest request)
    {
        try
        {
            if (!request.Body.CanRead || request.ContentLength == 0)
                return null;

            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true);

            var buffer = new char[MaxBodySize];
            var count = await reader.ReadBlockAsync(buffer, 0, MaxBodySize);
            var result = new string(buffer, 0, count);

            request.Body.Position = 0;

            return result.Length == 0 ? null : result;
        }
        catch
        {
            throw new Exception("Error reading request body");
        }
    }
}