using System.Text.Json;
using web_programming_lab_2.Exceptions;

namespace web_programming_lab_2.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            _logger.LogWarning(ex.Message, "Business exception: {Code}", ex.Code);
            await WriteErrorResponse(context, (int)ex.StatusCode, ex.Message, ex.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, 500, "An unexpected error occurred.", "INTERNAL_SERVER_ERROR");
        }
    }
    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message, string code)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new { error = message, code };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}