using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> AuthenticateAsync(AuthRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName)
                       ?? await _userManager.FindByEmailAsync(request.UserName);

            if (user == null || !user.IsActive || !user.EmailConfirmed)
                return new AuthResponse { HasError = true, Error = "Usuario no encontrado o no activado." };

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, false);
            if (!result.Succeeded)
                return new AuthResponse { HasError = true, Error = "Credenciales incorrectas." };

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateJwtToken(user.Id, user.UserName!, user.Email!, roles);

            return new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "",
                Token = token,
                IsActive = user.IsActive && user.EmailConfirmed
            };
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ConfirmAccountAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
                return true;
            }
            return false;
        }

        public async Task<bool> ResendActivationEmailAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName) ?? await _userManager.FindByEmailAsync(userName);
            if (user == null || user.EmailConfirmed) return false;

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var origin = _httpContextAccessor.HttpContext?.Request.Scheme + "://" + _httpContextAccessor.HttpContext?.Request.Host;
            var uri = new Uri($"{origin}/Account/ConfirmAccount?userId={user.Id}&token={Uri.EscapeDataString(token)}");

            await _emailService.SendEmailAsync(
                user.Email!,
                "Activación de cuenta - Artemis Banking Pro",
                $"<p>Hola <strong>{user.FirstName}</strong>,</p>" +
                $"<p>Solicitaste reenviar el enlace de activación de tu cuenta. Haz clic en el siguiente enlace para activar tu acceso:</p>" +
                $"<p style='margin-top:16px;'><a href='{uri}' style='display:inline-block;padding:10px 18px;background:#35648B;color:#fff;text-decoration:none;border-radius:8px;font-weight:bold;'>Activar mi Cuenta</a></p>");

            return true;
        }

        public async Task<string> GetResetTokenAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return string.Empty;
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;
            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            return result.Succeeded;
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
                    Identification = u.IdentificationNumber,
                    IsActive = u.IsActive && u.EmailConfirmed,
                    Role = roles.FirstOrDefault() ?? ""
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
                Identification = u.IdentificationNumber,
                IsActive = u.IsActive && u.EmailConfirmed,
                Role = roles.FirstOrDefault() ?? ""
            };
        }

        public async Task<bool> ChangeUserStatusAsync(string id, bool status)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            user.IsActive = status;
            user.EmailConfirmed = status;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserCascadeAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
