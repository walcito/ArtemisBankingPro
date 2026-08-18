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

        CreateMap<ForgotPasswordRequest, ForgotPasswordViewModel>()
            .ReverseMap();

        CreateMap<ResetPasswordRequest, ResetPasswordViewModel>()
            .ReverseMap();

        CreateMap<ConfirmAccountRequest, ConfirmAccountViewModel>()
            .ReverseMap();

        CreateMap<CreateUserDto, SaveUserViewModel>()
            .ReverseMap();

        CreateMap<CreateCommerceUserDto, SaveUserViewModel>()
            .ReverseMap();

        CreateMap<EditUserDto, SaveUserViewModel>()
            .ReverseMap();

        CreateMap<UserDto, UserViewModel>()
            .ReverseMap();

        CreateMap<UserDto, SaveUserViewModel>()
            .ReverseMap();
    }
}