using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
}
