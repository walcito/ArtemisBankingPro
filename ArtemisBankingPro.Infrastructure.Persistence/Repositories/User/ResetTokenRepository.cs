using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.User;

public class ResetTokenRepository : GenericRepository<ResetToken>, IResetTokenRepository
{
    private readonly ArtemisBankingDbContext _dbContext;

    public ResetTokenRepository(ArtemisBankingDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
