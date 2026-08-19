using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;

namespace ArtemisBankingPro.Core.Application.Interfaces.Card
{
    public interface ICreditCardService : IGenericService<CreditCard>
    {
        Task<CreditCardPagedResultDto> GetFilteredAsync(CreditCardFilterDto filter);
        Task<DetailCreditCardDto?> GetDetailAsync(int id);
        Task<(bool Success, string Message, CreateCreditCardResponseDto? Data)> AssignCardAsync(CreateCreditCardDto dto);
        Task<(bool Success, string Message, ListCreditCardDto? Data)> UpdateCreditLimitAsync(int cardId, UpdateCreditLimitRequestDto dto);
        Task<(bool Success, string Message)> CancelCardAsync(int cardId, string? reason);
    }
}
