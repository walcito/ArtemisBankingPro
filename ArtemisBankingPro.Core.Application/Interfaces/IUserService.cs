using ArtemisBankingPro.Core.Application.Dtos.User;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(string id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task UpdateAsync(UserDto dto, string id);
    Task DeleteAsync(string id);
    Task ChangeStatusAsync(string id, bool status);
}