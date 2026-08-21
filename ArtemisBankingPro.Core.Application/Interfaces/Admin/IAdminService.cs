using ArtemisBankingPro.Core.Application.Dtos.User;

namespace ArtemisBankingPro.Core.Application.Interfaces.Admin;

public interface IAdminService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task CreateUserAsync(CreateUserDto dto);
    Task EditUserAsync(string id, UserDto dto);
    Task ChangeUserStatusAsync(string id, bool status);
}
