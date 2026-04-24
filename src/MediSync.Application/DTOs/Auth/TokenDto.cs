namespace MediSync.Application.DTOs.Auth;

public record TokenDto(
    string   AccessToken,
    string   RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

public record UserInfo(
    Guid   Id,
    string FullName,
    string Email,
    string Role
);