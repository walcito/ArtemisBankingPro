 /*using Microsoft.AspNetCore.Identity;
using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Infrastructure.Identity.Entities;

namespace ArtemisBankingPro.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<AuthResponseDto?> LoginAsync(AuthRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                       ?? await _userManager.FindByNameAsync(request.Email);

            if (user == null || !user.IsActive)
                return null;

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, false);
            if (!result.Succeeded)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList(),
                IsVerified = user.EmailConfirmed
            };
        }

        public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterUserDto request, string role)
        {
            if (await UserExistsAsync(request.UserName))
                return (false, $"El nombre de usuario '{request.UserName}' ya está en uso.");

            if (await EmailExistsAsync(request.Email))
                return (false, $"El correo electrónico '{request.Email}' ya está registrado.");

            if (!await _roleManager.RoleExistsAsync(role))
                return (false, $"El rol '{role}' no existe.");

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IdentificationNumber = request.IdentificationNumber,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Error en el registro: {errors}");
            }

            await _userManager.AddToRoleAsync(user, role);
            return (true, $"Usuario registrado exitosamente con el rol '{role}'.");
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            var userList = new List<UserDto>();
            var users = await _userManager.GetUsersInRoleAsync(roleName);

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                userList.Add(new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? "",
                    UserName = u.UserName ?? "",
                    IdentificationNumber = u.IdentificationNumber,
                    PhoneNumber = u.PhoneNumber ?? "",
                    IsActive = u.IsActive,
                    Roles = roles.ToList()
                });
            }

            return userList;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return null;

            var roles = await _userManager.GetRolesAsync(u);

            return new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email ?? "",
                UserName = u.UserName ?? "",
                IdentificationNumber = u.IdentificationNumber,
                PhoneNumber = u.PhoneNumber ?? "",
                IsActive = u.IsActive,
                Roles = roles.ToList()
            };
        }

        public async Task<(bool Success, string Message)> ChangeUserStatusAsync(string id, bool status)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return (false, "Usuario no encontrado.");

            if (await _userManager.IsInRoleAsync(user, "Admin") && !status)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var activeAdmins = admins.Count(a => a.IsActive);
                if (activeAdmins <= 1)
                    return (false, "Debe existir al menos un administrador activo en el sistema.");
            }

            user.IsActive = status;
            await _userManager.UpdateAsync(user);

            return (true, $"El estado del usuario se actualizó a {(status ? "Activo" : "Inactivo")}.");
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return (false, "Usuario no encontrado.");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!resetResult.Succeeded)
                {
                    return (false, "Error al actualizar la contraseña del usuario.");
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return (false, "Error al actualizar los datos del usuario.");

            return (true, "Usuario actualizado correctamente.");
        }

        public async Task<(bool Success, string Message)> DeleteUserCascadeAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return (false, "Usuario no encontrado.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return (false, "Error al eliminar el usuario de la base de datos.");

            return (true, "Usuario eliminado correctamente.");
        }
    }
}*/