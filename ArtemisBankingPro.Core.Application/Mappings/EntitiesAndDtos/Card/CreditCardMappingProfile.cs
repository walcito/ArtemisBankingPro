using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos.Card
{
    public class CreditCardMappingProfile : Profile
    {
        public CreditCardMappingProfile()
        {
            #region CreditCard Mappings

            CreateMap<CreditCard, ListCreditCardDto>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.Nombre} {src.User.Apellido}" : string.Empty))
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)))
                .ForMember(dest => dest.LastFourDigits, opt => opt.MapFrom(src => GetLast4Digits(src.CardNumber)))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.ToString("MM/yy")));

            CreateMap<CreditCard, DetailCreditCardDto>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.Nombre} {src.User.Apellido}" : string.Empty))
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)))
                .ForMember(dest => dest.LastFourDigits, opt => opt.MapFrom(src => GetLast4Digits(src.CardNumber)))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.ToString("MM/yy")))
                .ForMember(dest => dest.Consumptions, opt => opt.MapFrom(src => src.Consumptions ?? new List<CardConsumption>()));

            CreateMap<CreditCard, CreateCreditCardResponseDto>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.Nombre} {src.User.Apellido}" : string.Empty))
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)))
                .ForMember(dest => dest.LastFourDigits, opt => opt.MapFrom(src => GetLast4Digits(src.CardNumber)))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.ToString("MM/yy")));

            CreateMap<CreateCreditCardDto, CreditCard>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CardNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CvcHash, opt => opt.Ignore())
                .ForMember(dest => dest.ExpirationDate, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentDebt, opt => opt.MapFrom(_ => 0m))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Common.Enums.Card.CardStatus.Active))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            #endregion
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
                return "****";

            return $"************{cardNumber[^4..]}";
        }

        private static string GetLast4Digits(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
                return "****";

            return cardNumber[^4..];
        }
    }
}