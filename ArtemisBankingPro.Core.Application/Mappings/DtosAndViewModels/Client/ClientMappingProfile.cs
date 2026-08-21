using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.ViewModels.Client;

namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels.Client;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        #region DTO → ViewModel Mappings
        CreateMap<ClientHomeDto, ClientHomeViewModel>().ReverseMap();
        CreateMap<ProductSummaryDto, ProductSummaryViewModel>().ReverseMap();
        CreateMap<BeneficiaryDto, BeneficiaryViewModel>().ReverseMap();
        #endregion
    }
}
