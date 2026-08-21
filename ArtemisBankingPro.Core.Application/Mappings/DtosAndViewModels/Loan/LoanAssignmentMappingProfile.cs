using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.ViewModels.Loan;

namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels.Loan;

public class LoanAssignmentMappingProfile : Profile
{
    public LoanAssignmentMappingProfile()
    {
        CreateMap<EligibleClientDto, EligibleClientViewModel>().ReverseMap();
    }
}
