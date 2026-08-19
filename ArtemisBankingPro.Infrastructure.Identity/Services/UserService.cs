using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _emailService = emailService;
        _httpContextAccessor = httpContextAccessor;
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
                IsActive = user.IsActive && user.EmailConfirmed
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
            IsActive = user.IsActive && user.EmailConfirmed
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
            EmailConfirmed = false,
            IsActive = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, dto.Role);

        // Generar Token de Activacion de correo
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var origin = _httpContextAccessor.HttpContext?.Request.Scheme + "://" + _httpContextAccessor.HttpContext?.Request.Host;
        var uri = new Uri($"{origin}/Account/ConfirmAccount?userId={user.Id}&token={Uri.EscapeDataString(token)}");

        await _emailService.SendEmailAsync(
            user.Email,
            "Activación de cuenta - Artemis Banking Pro",
            $"<p>Hola <strong>{user.FirstName}</strong>,</p>" +
            $"<p>Tu cuenta ha sido creada exitosamente. Para activarla y poder iniciar sesión en el sistema, haz clic en el siguiente enlace:</p>" +
            $"<p style='margin-top:16px;'><a href='{uri}' style='display:inline-block;padding:10px 18px;background:#35648B;color:#fff;text-decoration:none;border-radius:8px;font-weight:bold;'>Activar mi Cuenta</a></p>");

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
            user.IsActive = status;
            user.EmailConfirmed = status;
            await _userManager.UpdateAsync(user);
        }
    }
}