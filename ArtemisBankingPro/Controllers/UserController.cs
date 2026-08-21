using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces.Admin;
using ArtemisBankingPro.Core.Application.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.Controllers;

public class UserController : Controller
{
    private readonly IAdminService _adminService;

    public UserController(IAdminService adminService)
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
        return RedirectToAction("Index");
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

        var dto = new UserDto
        {
            Id = vm.Id ?? string.Empty,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Identification = vm.Identification,
            Email = vm.Email,
            UserName = vm.UserName,
            Role = vm.Role
        };

        await _adminService.EditUserAsync(vm.Id ?? string.Empty, dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(string id, bool status)
    {
        await _adminService.ChangeUserStatusAsync(id, status);
        return RedirectToAction("Index");
    }
}