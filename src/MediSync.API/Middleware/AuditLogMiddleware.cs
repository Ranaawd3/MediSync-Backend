using System.Security.Claims;
using MediSync.Application.Persistence;
using MediSync.Domain.Entities;

namespace MediSync.API.Middleware;

public class AuditLogMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx, IApplicationDbContext db)
    {
        await next(ctx);

        // سجّل الـ Mutations فقط — POST / PUT / DELETE
        if (ctx.Request.Method is not ("POST" or "PUT" or "DELETE"))
            return;

        // سجّل الـ Success فقط
        if (ctx.Response.StatusCode is < 200 or >= 300)
            return;

        var userIdStr = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);

        db.AuditLogs.Add(new AuditLog
        {
            UserId     = userIdStr != null ? Guid.Parse(userIdStr) : null,
            Action     = ctx.Request.Method,
            EntityType = ctx.Request.Path.Value ?? "",
            IpAddress  = ctx.Connection.RemoteIpAddress?.ToString(),
            CreatedAt  = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
    }
}