using MediatR;
using MediSync.Application.Features.Auth.Commands;
using Microsoft.EntityFrameworkCore;
using MediSync.Application.Persistence;

namespace MediSync.Application.Features.Auth.Handlers;

public class ForgotPasswordHandler(IApplicationDbContext db) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

        // لو الإيميل مش موجود — مش بنرجع error عشان Security
        if (user == null) return;

        // توليد Reset Token — صالح ساعة
        var resetToken = Convert.ToBase64String(
            System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

        user.RefreshToken      = $"reset_{resetToken}";
        user.RefreshTokenExpiry = DateTime.UtcNow.AddHours(1);
        await db.SaveChangesAsync(ct);

        // TODO: استخدمي IEmailService لما تعمليه في أسبوع 7
        // await emailService.SendResetEmail(user.Email, resetToken);
    }
}