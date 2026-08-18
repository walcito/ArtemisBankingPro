using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingProApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.AuthenticateAsync(request);

        if (result.HasError)
            return Unauthorized(new { HasError = true, Error = result.Error });

        return Ok(result);
    }
}
