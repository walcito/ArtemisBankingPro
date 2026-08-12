using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.Card
{
    public class CreditCardRepository : GenericRepository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<CreditCard?> GetByCardNumberAsync(string cardNumber)
        {
            return await _dbContext.Set<CreditCard>()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
        }

        public async Task<IReadOnlyList<CreditCard>> GetByUserIdAsync(int userId)
        {
            return await _dbContext.Set<CreditCard>()
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            return await _dbContext.Set<CreditCard>()
                .AnyAsync(c => c.CardNumber == cardNumber);
        }
    }
}