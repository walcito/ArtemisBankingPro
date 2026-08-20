using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.User;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(ArtemisBankingDbContext dbContext) : base(dbContext)
    {
    }
}
