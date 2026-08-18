using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos;

public class UserEntityProfile : Profile
{
    public UserEntityProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.IdentificationNumber))
            .ForMember(dest => dest.CommerceId, opt => opt.MapFrom(src => src.TradeId))
            .ReverseMap();

        CreateMap<User, CreateUserDto>()
            .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.IdentificationNumber))
            .ReverseMap();

        CreateMap<User, CreateCommerceUserDto>()
            .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.IdentificationNumber))
            .ReverseMap();

        CreateMap<User, EditUserDto>()
            .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.IdentificationNumber))
            .ReverseMap();
    }
}