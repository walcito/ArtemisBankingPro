using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos.SavingsAccount;

public class SavingsAccountEntityProfile : Profile
{
    public SavingsAccountEntityProfile()
    {
        CreateMap<Domain.Entities.SavingsAccounts.SavingsAccount, SavingsAccountDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src =>
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty))
            .ForMember(dest => dest.UserIdentificationNumber, opt => opt.MapFrom(src =>
                src.User != null ? src.User.IdentificationNumber : string.Empty));

        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                src.Type.ToString()));

        CreateMap<Beneficiary, Dtos.Client.BeneficiaryDto>();
    }
}
