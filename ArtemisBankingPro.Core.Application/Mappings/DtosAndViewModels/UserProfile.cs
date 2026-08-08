using AutoMapper;
using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.ViewModels.Account;
using ArtemisBankingPro.Core.Application.ViewModels.User;

namespace ArtemisBankingPro.Core.Application.Mappings.DtosAndViewModels;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AuthRequest, LoginViewModel>()
            .ReverseMap();

        CreateMap<ResetPasswordRequest, ResetPasswordViewModel>()
            .ReverseMap();

        //pendiente descomentar cuando se cree el dto de creacion de usuario waldin
        /* CreateMap<CreateUserDto, SaveUserViewModel>()
             .ReverseMap();*/

        CreateMap<UserDto, UserViewModel>()
            .ReverseMap();
    }
}