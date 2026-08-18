using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Identification = user.IdentificationNumber,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsActive = user.EmailConfirmed
            });
        }

        return userDtos;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Identification = user.IdentificationNumber,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.EmailConfirmed
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IdentificationNumber = dto.Identification,
            Email = dto.Email,
            UserName = dto.UserName,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, dto.Role);

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Identification = user.IdentificationNumber,
            Email = user.Email,
            UserName = user.UserName,
            Role = dto.Role,
            IsActive = false
        };
    }

    public async Task UpdateAsync(UserDto dto, string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) throw new Exception("Usuario no encontrado.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.IdentificationNumber = dto.Identification;
        user.Email = dto.Email;
        user.UserName = dto.UserName;

        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is not null)
            await _userManager.DeleteAsync(user);
    }

    public async Task ChangeStatusAsync(string id, bool status)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is not null)
        {
            user.EmailConfirmed = status;
            await _userManager.UpdateAsync(user);
        }
    }
}