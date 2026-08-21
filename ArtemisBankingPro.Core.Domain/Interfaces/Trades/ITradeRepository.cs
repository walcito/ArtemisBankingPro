using ArtemisBankingPro.Core.Domain.Entities.Trades;

namespace ArtemisBankingPro.Core.Domain.Interfaces.Trades
{
    public interface ITradeRepository : IGenericRepository<Trade>
    {
        Task<Trade?> GetByRncAsync(string rnc);
        Task<Trade?> GetByEmailAsync(string email);
        Task<bool> RncExistsAsync(string rnc);
        Task<bool> EmailExistsAsync(string email);
        Task<Trade?> GetByIdWithUserAsync(int id);
    }
}
