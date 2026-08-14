
namespace ArtemisBankingPro.Core.Application.Interfaces.Card
{
    public interface ICardNumberGeneratorService
    {
        Task<string> GenerateUniqueCardNumberAsync();
    }
}
