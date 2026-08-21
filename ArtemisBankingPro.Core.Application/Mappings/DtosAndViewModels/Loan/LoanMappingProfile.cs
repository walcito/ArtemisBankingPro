using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.ViewModels.Loan;

namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels.Loan;

public class LoanMappingProfile : Profile
{
    public LoanMappingProfile()
    {
        #region DTO → ViewModel Mappings
        CreateMap<LoanDto, LoanViewModel>().ReverseMap();
        CreateMap<DetallePrestamoDto, LoanDetailViewModel>().ReverseMap();
        CreateMap<CuotaPrestamoDto, LoanInstallmentViewModel>().ReverseMap();
        #endregion
    }
}
