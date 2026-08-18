using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using UserEntity = ArtemisBankingPro.Core.Domain.Entities.Users.User;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.User;

public class UserRepository : GenericRepository<UserEntity>, IUserRepository
{
    public UserRepository(ArtemisBankingDbContext dbContext) : base(dbContext)
    {
    }
}
