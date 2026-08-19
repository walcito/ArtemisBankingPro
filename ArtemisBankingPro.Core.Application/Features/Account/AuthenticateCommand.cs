using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Account;

public class AuthenticateCommand : IRequest<AuthResponse>
{
    public AuthRequest Request { get; set; } = new();
}

public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, AuthResponse>
{
    private readonly IAccountService _accountService;

    public AuthenticateCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<AuthResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.AuthenticateAsync(request.Request);
    }
}