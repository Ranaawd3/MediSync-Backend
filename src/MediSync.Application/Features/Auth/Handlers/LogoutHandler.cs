using MediatR;
using MediSync.Application.Features.Auth.Commands;
using Microsoft.EntityFrameworkCore;
using MediSync.Application.Persistence;

namespace MediSync.Application.Features.Auth.Handlers;

public class LogoutHandler(IApplicationDbContext db) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == req.RefreshToken, ct);

        if (user == null) return;

        user.RefreshToken       = null;
        user.RefreshTokenExpiry = null;
        await db.SaveChangesAsync(ct);
    }
}