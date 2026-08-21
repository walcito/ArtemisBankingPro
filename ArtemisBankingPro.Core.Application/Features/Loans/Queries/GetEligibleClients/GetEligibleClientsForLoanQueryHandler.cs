using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Common.Enums.Users;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetEligibleClients;

public class GetEligibleClientsForLoanQueryHandler
    : IRequestHandler<GetEligibleClientsForLoanQuery, EligibleClientsResultDto>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IGenericRepository<CreditCard> _creditCardRepository;

    public GetEligibleClientsForLoanQueryHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<Loan> loanRepository,
        IGenericRepository<CreditCard> creditCardRepository)
    {
        _userRepository = userRepository;
        _loanRepository = loanRepository;
        _creditCardRepository = creditCardRepository;
    }

    public async Task<EligibleClientsResultDto> Handle(
        GetEligibleClientsForLoanQuery request, CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetAllAsync();
        var activeClients = allUsers
            .Where(u => u.Role == Roles.Client && u.IsActive)
            .ToList();

        var allLoans = await _loanRepository.GetAllAsync();
        var activeLoans = allLoans.Where(l => l.Status == LoanStatus.Active).ToList();

        // IDs de clientes que ya tienen un préstamo activo
        var clientsWithActiveLoan = activeLoans.Select(l => l.UserId).ToHashSet();

        var allCards = await _creditCardRepository.GetAllAsync();
        var activeCards = allCards
            .Where(c => c.Status == Domain.Common.Enums.Card.CardStatus.Active)
            .ToList();

        // Clientes elegibles: activos, rol Cliente, sin préstamo activo
        var eligibleClients = activeClients
            .Where(u => !clientsWithActiveLoan.Contains(u.Id))
            .ToList();

        // Filtrar por cédula si se proporcionó
        if (!string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            eligibleClients = eligibleClients
                .Where(u => u.IdentificationNumber.Contains(request.IdentificationNumber.Trim()))
                .ToList();
        }

        // Calcular deuda total por cliente (préstamos activos + tarjetas activas)
        var result = eligibleClients.Select(u =>
        {
            decimal loanDebt = activeLoans
                .Where(l => l.UserId == u.Id)
                .Sum(l => l.CurrentDebt);
            decimal cardDebt = activeCards
                .Where(c => c.UserId == u.Id)
                .Sum(c => c.CurrentDebt);

            return new EligibleClientDto
            {
                Id = u.Id,
                IdentificationNumber = u.IdentificationNumber,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                TotalDebt = loanDebt + cardDebt
            };
        }).ToList();

        // Deuda promedio de todos los clientes activos del sistema
        decimal averageDebt = 0;
        if (activeClients.Any())
        {
            decimal totalSystemDebt = activeClients.Sum(u =>
            {
                decimal ld = activeLoans.Where(l => l.UserId == u.Id).Sum(l => l.CurrentDebt);
                decimal cd = activeCards.Where(c => c.UserId == u.Id).Sum(c => c.CurrentDebt);
                return ld + cd;
            });
            averageDebt = totalSystemDebt / activeClients.Count;
        }

        return new EligibleClientsResultDto
        {
            Clients = result,
            AverageDebt = Math.Round(averageDebt, 2)
        };
    }
}
