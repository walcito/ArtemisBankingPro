using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.ViewModels.SavingsAccount;

namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels.SavingsAccount;

public class SavingsAccountMappingProfile : Profile
{
    public SavingsAccountMappingProfile()
    {
        #region DTO → ViewModel Mappings
        CreateMap<SavingsAccountDto, SavingsAccountViewModel>().ReverseMap();
        CreateMap<TransactionDto, TransactionViewModel>().ReverseMap();
        #endregion
    }
}
