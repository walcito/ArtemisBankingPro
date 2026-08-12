using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos.Card
{
    public class CardConsumptionMappingProfile : Profile
    {
        public CardConsumptionMappingProfile()
        {
            #region CardConsumption Mappings

            CreateMap<CardConsumption, CardConsumptionDto>()
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => src.CreditCard != null ? MaskCardNumber(src.CreditCard.CardNumber) : string.Empty));

            CreateMap<CardConsumption, ConsumptionDetailDto>();

            #endregion
        }
    
    private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
                return "****";

            return $"************{cardNumber[^4..]}";
        }
    }
}
