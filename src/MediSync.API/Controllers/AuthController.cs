using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediSync.Application.DTOs.Auth;
using MediSync.Application.DTOs.Common;
using MediSync.Application.Features.Auth.Commands;
using Microsoft.EntityFrameworkCore;
using MediSync.Infrastructure.Persistence;
using MediSync.Application.Services;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IMediator mediator, AppDbContext db, ITokenService tokenService, IConfiguration config)
    : ControllerBase
{
    /// <summary>تسجيل مستخدم جديد</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await mediator.Send(new RegisterCommand(dto));
        return StatusCode(201, ApiResponse<TokenDto>.Ok(result, "تم التسجيل بنجاح"));
    }

    /// <summary>تسجيل الدخول</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await mediator.Send(new LoginCommand(dto));
        return Ok(ApiResponse<TokenDto>.Ok(result));
    }

    /// <summary>تجديد الـ Token</summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == req.RefreshToken &&
                 u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
            return Unauthorized(ApiResponse<object>.Fail("AUTH_REFRESH_EXPIRED"));

        var newRefresh = tokenService.GenerateRefreshToken();
        user.RefreshToken       = newRefresh;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            int.Parse(config["Jwt:RefreshTokenDays"]!));
        await db.SaveChangesAsync();

        var accessToken = tokenService.GenerateAccessToken(user);
        var expiresAt   = DateTime.UtcNow.AddMinutes(
            int.Parse(config["Jwt:AccessTokenMinutes"]!));

        var result = new TokenDto(accessToken, newRefresh, expiresAt,
            new UserInfo(user.Id, user.FullName, user.Email, user.Role));
        return Ok(ApiResponse<TokenDto>.Ok(result));
    }

    /// <summary>تسجيل الخروج</summary>
    [HttpPost("logout"), Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
    {
        await mediator.Send(new LogoutCommand(req.RefreshToken));
        return Ok(ApiResponse<object>.Ok(new {}, "تم تسجيل الخروج"));
    }

    /// <summary>إرسال رابط استعادة كلمة المرور</summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
    {
        await mediator.Send(new ForgotPasswordCommand(req.Email));
        return Ok(ApiResponse<object>.Ok(new {}, "لو الإيميل موجود، هيوصلك رابط"));
    }

/// <summary>إعادة تعيين كلمة المرور</summary>
[HttpPut("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
{
    await mediator.Send(new ResetPasswordCommand(req.Token, req.NewPassword));
    return Ok(ApiResponse<object>.Ok(new {}, "تم تغيير كلمة المرور"));
}

}

public record RefreshRequest(string RefreshToken);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);