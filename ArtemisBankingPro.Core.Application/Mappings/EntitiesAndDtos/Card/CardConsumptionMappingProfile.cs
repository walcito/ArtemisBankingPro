using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using AutoMapper;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos.Card
{
    public class CardConsumptionMappingProfile : Profile
    {
        public CardConsumptionMappingProfile()
        {
            CreateMap<CardConsumption, CardConsumptionDto>()
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => src.CreditCard != null && src.CreditCard.CardNumber.Length >= 4 ? $"************{src.CreditCard.CardNumber[4]}" : "****************"))
                .ForMember(dest => dest.CommerceName, opt => opt.MapFrom(src => src.Description));

            CreateMap<CardConsumption, ConsumptionDetailDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.CommerceName, opt => opt.MapFrom(src => src.Description));
        }
    }
}
