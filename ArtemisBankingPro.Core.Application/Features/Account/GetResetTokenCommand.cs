using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Account;

public class GetResetTokenCommand : IRequest<string>
{
    public ForgotPasswordRequest Request { get; set; } = new();
}

public class GetResetTokenCommandHandler : IRequestHandler<GetResetTokenCommand, string>
{
    private readonly IAccountService _accountService;

    public GetResetTokenCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<string> Handle(GetResetTokenCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.GetResetTokenAsync(request.Request.UserName);
    }
}