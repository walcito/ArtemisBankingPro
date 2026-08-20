using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Account;

public class ResetPasswordCommand : IRequest<bool>
{
    public ResetPasswordRequest Request { get; set; } = new();
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly IAccountService _accountService;

    public ResetPasswordCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.ResetPasswordAsync(request.Request);
    }
}