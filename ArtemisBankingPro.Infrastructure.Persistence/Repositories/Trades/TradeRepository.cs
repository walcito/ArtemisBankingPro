using ArtemisBankingPro.Core.Domain.Entities.Trades;
using ArtemisBankingPro.Core.Domain.Interfaces.Trades;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.Trades
{
    public class TradeRepository : GenericRepository<Trade>, ITradeRepository
    {
        public TradeRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<Trade?> GetByRncAsync(string rnc)
        {
            return await _dbContext.Set<Trade>()
                .Include(t => t.AssociatedUser)
                .FirstOrDefaultAsync(t => t.Rnc == rnc);
        }

        public async Task<Trade?> GetByEmailAsync(string email)
        {
            return await _dbContext.Set<Trade>()
                .Include(t => t.AssociatedUser)
                .FirstOrDefaultAsync(t => t.Mail == email);
        }

        public async Task<bool> RncExistsAsync(string rnc)
        {
            return await _dbContext.Set<Trade>()
                .AnyAsync(t => t.Rnc == rnc);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Set<Trade>()
                .AnyAsync(t => t.Mail == email);
        }

        public async Task<Trade?> GetByIdWithUserAsync(int id)
        {
            return await _dbContext.Set<Trade>()
                .Include(t => t.AssociatedUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
