using Domain.Models.Entities;

namespace Application.Servicies.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    public string CreateRefreshToken();
}