using MediatR;
using MediSync.Application.DTOs.Auth;
using MediSync.Application.Features.Auth.Commands;
using MediSync.Application.Services;
using MediSync.Domain.Entities;
using MediSync.Application.Persistence;
using Microsoft.Extensions.Configuration;

namespace MediSync.Application.Features.Auth.Handlers;

public class RegisterHandler(
    IApplicationDbContext db,
    ITokenService  tokenService,
    IConfiguration config)
    : IRequestHandler<RegisterCommand, TokenDto>
{
    public async Task<TokenDto> Handle(RegisterCommand req, CancellationToken ct)
    {
        var dto = req.Dto;

        if (db.Users.Any(u => u.Email == dto.Email))
            throw new InvalidOperationException("البريد الإلكتروني مستخدم بالفعل");

        var refreshToken  = tokenService.GenerateRefreshToken();
        var refreshDays   = int.Parse(config["Jwt:RefreshTokenDays"]!);

        var user = new User {
            Email               = dto.Email,
            PasswordHash        = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName            = dto.FullName,
            Age                 = dto.Age,
            WeightKg            = dto.WeightKg,
            Gender              = dto.Gender,
            ChronicConditions   = dto.ChronicConditions,
            Allergies           = dto.Allergies,
            RefreshToken        = refreshToken,
            RefreshTokenExpiry  = DateTime.UtcNow.AddDays(refreshDays)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var accessToken = tokenService.GenerateAccessToken(user);
        var expiresAt   = DateTime.UtcNow.AddMinutes(
                            int.Parse(config["Jwt:AccessTokenMinutes"]!));

        return new TokenDto(
            accessToken, refreshToken, expiresAt,
            new UserInfo(user.Id, user.FullName, user.Email, user.Role));
    }
}