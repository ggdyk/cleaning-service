using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int? ValidateAccessToken(string token);
}