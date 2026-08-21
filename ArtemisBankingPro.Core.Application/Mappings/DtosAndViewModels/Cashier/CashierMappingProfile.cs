using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Cashier;
using ArtemisBankingPro.Core.Application.ViewModels.Cashier;

namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels.Cashier;

public class CashierMappingProfile : Profile
{
    public CashierMappingProfile()
    {
        #region DTO → ViewModel Mappings
        CreateMap<CashierHomeDto, CashierHomeViewModel>().ReverseMap();
        #endregion
    }
}
