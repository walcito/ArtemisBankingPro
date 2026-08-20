using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    string GenerateJwtToken(string id, string userName, string email, IList<string> roles);
}
