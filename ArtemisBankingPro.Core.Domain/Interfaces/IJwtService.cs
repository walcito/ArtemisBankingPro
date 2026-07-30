using ArtemisBankingPro.Core.Domain.Entities;

namespace ArtemisBankingPro.Core.Domain.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
}
