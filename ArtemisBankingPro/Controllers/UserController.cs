using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Features.Users;
using ArtemisBankingPro.Core.Application.ViewModels.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.Controllers;

public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _mediator.Send(new GetUsersQuery());
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

        await _mediator.Send(new CreateUserCommand { User = dto });
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var users = await _mediator.Send(new GetUsersQuery());
        var user = users.FirstOrDefault(u => u.Id == id);
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

        await _mediator.Send(new EditUserCommand { Id = vm.Id ?? string.Empty, User = dto });
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(string id, bool status)
    {
        await _mediator.Send(new ChangeUserStatusCommand { Id = id, Status = status });
        return RedirectToAction("Index");
    }
}