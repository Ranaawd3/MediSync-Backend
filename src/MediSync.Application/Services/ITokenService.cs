using MediSync.Domain.Entities;

namespace MediSync.Application.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid?  ValidateAccessToken(string token);
}