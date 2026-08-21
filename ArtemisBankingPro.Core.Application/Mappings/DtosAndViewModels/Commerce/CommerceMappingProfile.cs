using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Commerce;
using ArtemisBankingPro.Core.Application.ViewModels.Commerce;

namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels.Commerce;

public class CommerceMappingProfile : Profile
{
    public CommerceMappingProfile()
    {
        #region DTO → ViewModel Mappings
        CreateMap<TradeDto, CommerceViewModel>().ReverseMap();
        CreateMap<TradeDetailDto, CommerceDetailViewModel>().ReverseMap();
        #endregion
    }
}
