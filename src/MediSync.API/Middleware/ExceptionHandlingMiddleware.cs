using System.Net;
using System.Text.Json;

namespace MediSync.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try {
            await next(ctx);
        }
        catch (UnauthorizedAccessException ex) {
            await WriteError(ctx, 401, ex.Message);
        }
        catch (InvalidOperationException ex) {
            await WriteError(ctx, 400, ex.Message);
        }
        catch (KeyNotFoundException ex) {
            await WriteError(ctx, 404, ex.Message);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Unhandled exception");
            await WriteError(ctx, 500, "حدث خطأ داخلي");
        }
    }

    private static async Task WriteError(HttpContext ctx, int status, string message)
    {
        ctx.Response.StatusCode  = status;
        ctx.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new {
            success = false,
            error   = new { code = message, message }
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await ctx.Response.WriteAsync(body);
    }
}