using Microsoft.Extensions.Caching.Distributed;

namespace MediSync.API.Middleware;

public class RateLimitingMiddleware(RequestDelegate next)
{
    private const int MaxPerMinute = 100;

    public async Task InvokeAsync(HttpContext ctx, IDistributedCache cache)
    {
        var ip  = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rl:{ip}:{DateTime.UtcNow:yyyyMMddHHmm}";

        int count = 0;
        var cached = await cache.GetStringAsync(key);
        if (cached != null) int.TryParse(cached, out count);

        if (count >= MaxPerMinute)
        {
            ctx.Response.StatusCode  = 429;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync("""
                {"success":false,"error":{"code":"RATE_LIMIT_EXCEEDED",
                "message":"تجاوزت الحد المسموح — حاول بعد دقيقة"}}
                """);
            return;
        }

        await cache.SetStringAsync(key, (count + 1).ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

        await next(ctx);
    }
}