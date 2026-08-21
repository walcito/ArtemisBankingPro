using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _adminService.GetAllUsersAsync();
        var vms = users.Select(u => new UserViewModel
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Identification = u.Identification,
            Email = u.Email,
            UserName = u.UserName,
            Role = u.Role,
            IsActive = u.IsActive
        }).ToList();

        return View(vms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new SaveUserViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveUserViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            if (vm.Role == "Commerce")
            {
                var commerceDto = new CreateCommerceUserDto
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    Identification = vm.Identification,
                    Email = vm.Email,
                    UserName = vm.UserName,
                    Password = vm.Password ?? string.Empty,
                    ConfirmPassword = vm.ConfirmPassword ?? string.Empty,
                    InitialAmount = vm.InitialAmount
                };

                await _adminService.CreateUserAsync(new CreateUserDto
                {
                    FirstName = commerceDto.FirstName,
                    LastName = commerceDto.LastName,
                    Identification = commerceDto.Identification,
                    Email = commerceDto.Email,
                    UserName = commerceDto.UserName,
                    Password = commerceDto.Password,
                    ConfirmPassword = commerceDto.ConfirmPassword,
                    Role = "Commerce",
                    InitialAmount = commerceDto.InitialAmount
                });
            }
            else
            {
                var dto = new CreateUserDto
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    Identification = vm.Identification,
                    Email = vm.Email,
                    UserName = vm.UserName,
                    Password = vm.Password ?? string.Empty,
                    ConfirmPassword = vm.ConfirmPassword ?? string.Empty,
                    Role = vm.Role,
                    InitialAmount = vm.InitialAmount
                };

                await _adminService.CreateUserAsync(dto);
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            vm.HasError = true;
            vm.Error = ex.Message;
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _adminService.GetUserByIdAsync(id);
        if (user is null) return RedirectToAction("Index");

        var vm = new SaveUserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Identification = user.Identification,
            Email = user.Email,
            UserName = user.UserName,
            Role = user.Role
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SaveUserViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var userDto = new UserDto
            {
                Id = vm.Id ?? string.Empty,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Identification = vm.Identification,
                Email = vm.Email,
                UserName = vm.UserName,
                Role = vm.Role
            };

            await _adminService.EditUserAsync(vm.Id ?? string.Empty, userDto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            vm.HasError = true;
            vm.Error = ex.Message;
            return View(vm);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(string id, bool status)
    {
        await _adminService.ChangeUserStatusAsync(id, status);
        return RedirectToAction("Index");
    }
}