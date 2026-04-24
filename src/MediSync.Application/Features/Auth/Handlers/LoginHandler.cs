using MediatR;
using MediSync.Application.DTOs.Auth;
using MediSync.Application.Features.Auth.Commands;
using MediSync.Application.Services;
using Microsoft.EntityFrameworkCore;
using MediSync.Application.Persistence;
using Microsoft.Extensions.Configuration;

namespace MediSync.Application.Features.Auth.Handlers;

public class LoginHandler(
    IApplicationDbContext db,
    ITokenService  tokenService,
    IConfiguration config)
    : IRequestHandler<LoginCommand, TokenDto>
{
    public async Task<TokenDto> Handle(LoginCommand req, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Dto.Email, ct)
            ?? throw new UnauthorizedAccessException("AUTH_INVALID_CREDENTIALS");

        if (!BCrypt.Net.BCrypt.Verify(req.Dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("AUTH_INVALID_CREDENTIALS");

        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshDays  = int.Parse(config["Jwt:RefreshTokenDays"]!);

        user.RefreshToken       = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);
        await db.SaveChangesAsync(ct);

        var accessToken = tokenService.GenerateAccessToken(user);
        var expiresAt   = DateTime.UtcNow.AddMinutes(
                            int.Parse(config["Jwt:AccessTokenMinutes"]!));

        return new TokenDto(
            accessToken, refreshToken, expiresAt,
            new UserInfo(user.Id, user.FullName, user.Email, user.Role));
    }
}