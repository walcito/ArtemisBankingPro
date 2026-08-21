using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Commerce;
using ArtemisBankingPro.Core.Domain.Entities.Trades;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos.Commerce;

public class CommerceEntityProfile : Profile
{
    public CommerceEntityProfile()
    {
        CreateMap<Trade, TradeDto>();

        CreateMap<Trade, TradeDetailDto>()
            .ForMember(dest => dest.AssociatedUsername, opt => opt.MapFrom(src =>
                src.AssociatedUser != null ? src.AssociatedUser.Username : null));
    }
}
