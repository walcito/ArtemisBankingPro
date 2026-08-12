using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Application.ViewModels.Card;
using AutoMapper;


namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels.Card
{
    public class CreditCardDtoMappingProfile : Profile
    {
        public CreditCardDtoMappingProfile()
        {
            #region DTO -- ViewModel Mappings
            CreateMap<ListCreditCardDto, CreditCardViewModel>().ReverseMap();
            
            CreateMap<DetailCreditCardDto, CreditCardDetailsViewModel>().ReverseMap();
            CreateMap<ConsumptionDetailDto, CardConsumptionViewModel>().ReverseMap();
        
            CreateMap<AssignCreditCardViewModel, CreateCreditCardDto>();
       
            CreateMap<UpdateCreditLimitViewModel, UpdateCreditLimitRequestDto>()
                .ForMember(dest => dest.CreditLimit, opt => opt.MapFrom(src => src.NewCreditLimit));
      
            CreateMap<CreditCardPagedResultDto, CreditCardListFilterViewModel>()
                .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Data));
            #endregion
        }
    }
}
