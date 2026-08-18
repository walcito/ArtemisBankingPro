using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ArtemisBankingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
