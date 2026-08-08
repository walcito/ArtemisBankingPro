using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Dtos.User;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface IAccountService
{
    Task<AuthResponse> AuthenticateAsync(AuthRequest request);
    Task<bool> ConfirmAccountAsync(string token);
    Task<string> GetResetTokenAsync(string userName);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> UserExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<List<UserDto>> GetUsersByRoleAsync(string roleName);
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<bool> ChangeUserStatusAsync(string id, bool status);
    Task<bool> DeleteUserCascadeAsync(string id);
}