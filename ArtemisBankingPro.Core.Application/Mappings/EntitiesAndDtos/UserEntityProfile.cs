using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Application.Mappings.EntitiesAndDtos;

public class UserEntityProfile : Profile
{
    public UserEntityProfile()
    {
        CreateMap<User, UserDto>()
            .ReverseMap();

        CreateMap<User, CreateUserDto>()
            .ReverseMap();
    }
}