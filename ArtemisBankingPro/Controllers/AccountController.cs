using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Features.Account;
using ArtemisBankingPro.Core.Application.ViewModels.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.Controllers;

public class AccountController : Controller
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _mediator.Send(new AuthenticateCommand
        {
            Request = new AuthRequest
            {
                UserName = vm.UserName,
                Password = vm.Password
            }
        });

        if (result.HasError)
        {
            vm.HasError = true;
            vm.Error = result.Error ?? "Error al iniciar sesión.";
            return View(vm);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmAccount(string token)
    {
        var request = new ConfirmAccountRequest { Token = token };
        var success = await _mediator.Send(new ConfirmAccountCommand { Request = request });

        var vm = new ConfirmAccountViewModel
        {
            Success = success,
            Message = success ? "Cuenta activada exitosamente." : "El token de activación no es válido o ha expirado."
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var request = new ForgotPasswordRequest { UserName = vm.UserName };
        await _mediator.Send(new GetResetTokenCommand { Request = request });

        ViewBag.Message = "Se ha enviado un correo con las instrucciones para restablecer su contraseña.";
        return View(vm);
    }

    [HttpGet]
    public IActionResult ResetPassword(string userId, string token)
    {
        return View(new ResetPasswordViewModel { UserId = userId, Token = token });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var success = await _mediator.Send(new ResetPasswordCommand
        {
            Request = new ResetPasswordRequest
            {
                UserId = vm.UserId,
                Token = vm.Token,
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword
            }
        });

        if (!success)
        {
            vm.HasError = true;
            vm.Error = "No se pudo restablecer la contraseña. Verifique que el token sea válido.";
            return View(vm);
        }

        return RedirectToAction("Login");
    }
}
