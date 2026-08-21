using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Domain.Entities.Loans;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos.Loan;

public class LoanEntityProfile : Profile
{
    public LoanEntityProfile()
    {
        CreateMap<Domain.Entities.Loans.Loan, LoanDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.UserIdentificationNumber, opt => opt.MapFrom(src =>
                src.User != null ? src.User.IdentificationNumber : string.Empty));

        CreateMap<Domain.Entities.Loans.Loan, LoanDetailDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.UserIdentificationNumber, opt => opt.MapFrom(src => src.User != null ? src.User.IdentificationNumber : string.Empty))
            .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.Installments.OrderBy(i => i.InstallmentNumber)));

        CreateMap<AmortizationInstallment, LoanInstallmentDto>();
    }
}
