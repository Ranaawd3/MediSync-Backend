using MediatR;
using MediSync.Application.Features.Auth.Commands;
using Microsoft.EntityFrameworkCore;
using MediSync.Application.Persistence;

namespace MediSync.Application.Features.Auth.Handlers;

public class ResetPasswordHandler(IApplicationDbContext db) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand req, CancellationToken ct)
    {
        var tokenStored = $"reset_{req.Token}";
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == tokenStored &&
                 u.RefreshTokenExpiry > DateTime.UtcNow, ct)
            ?? throw new KeyNotFoundException("الرابط منتهي أو غير صحيح");

        user.PasswordHash      = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.RefreshToken      = null;
        user.RefreshTokenExpiry = null;
        await db.SaveChangesAsync(ct);
    }
}