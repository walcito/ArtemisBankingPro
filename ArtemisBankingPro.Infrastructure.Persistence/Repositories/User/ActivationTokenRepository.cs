using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.User;

public class ActivationTokenRepository : GenericRepository<ActivationToken>, IActivationTokenRepository
{
    private readonly ArtemisBankingDbContext _dbContext;

    public ActivationTokenRepository(ArtemisBankingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
