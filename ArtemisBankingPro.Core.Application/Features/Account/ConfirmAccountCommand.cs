using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Account;

public class ConfirmAccountCommand : IRequest<bool>
{
    public ConfirmAccountRequest Request { get; set; } = new();
}

public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand, bool>
{
    private readonly IAccountService _accountService;

    public ConfirmAccountCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<bool> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.ConfirmAccountAsync(request.Request.Token);
    }
}