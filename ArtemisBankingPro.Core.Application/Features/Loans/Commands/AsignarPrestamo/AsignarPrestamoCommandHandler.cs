using ArtemisBankingPro.Core.Application.Features.Loans.Commands.DesembolsarPrestamo;
using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.AsignarPrestamo;

public class AsignarPrestamoCommandHandler : IRequestHandler<AsignarPrestamoCommand, int>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IAmortizationService _amortizationService;
    private readonly GeneradorNumeroPrestamoService _generadorNumeroPrestamo;
    private readonly IMediator _mediator;

    public AsignarPrestamoCommandHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<Loan> loanRepository,
        IAmortizationService amortizationService,
        GeneradorNumeroPrestamoService generadorNumeroPrestamo,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _loanRepository = loanRepository;
        _amortizationService = amortizationService;
        _generadorNumeroPrestamo = generadorNumeroPrestamo;
        _mediator = mediator;
    }

    public async Task<int> Handle(AsignarPrestamoCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null || !user.IsActive)
            throw new Exception("El cliente no existe o no está activo.");

        var loans = await _loanRepository.GetAllAsync();
        var hasActiveLoan = loans.Any(l => l.UserId == request.UserId && l.Status == LoanStatus.Active);

        if (hasActiveLoan)
            throw new Exception("El cliente ya tiene un préstamo activo.");

        var loanNumber = await _generadorNumeroPrestamo.GenerarNumeroUnicoAsync();

        var installments = _amortizationService.GenerateAmortizationTable(
            request.DisbursedAmount,
            request.AnnualInterestRate,
            request.TermInMonths
        );

        // Validación de Riesgo: Deuda Actual Cliente + Total a Pagar Nuevo Préstamo > Deuda Promedio Global
        var activeLoans = loans.Where(l => l.Status == LoanStatus.Active).ToList();
        if (activeLoans.Any())
        {
            decimal clientCurrentDebt = activeLoans.Where(l => l.UserId == request.UserId).Sum(l => l.CurrentDebt);
            decimal newLoanTotalToPay = installments.Sum(i => i.InstallmentAmount);
            decimal globalAverageDebt = activeLoans.Average(l => l.CurrentDebt);

            if (clientCurrentDebt + newLoanTotalToPay > globalAverageDebt)
            {
                if (!request.ConfirmHighRisk)
                {
                    throw new ArtemisBankingPro.Core.Application.Exceptions.ClienteAltoRiesgoException(
                        "El préstamo representa un alto riesgo financiero. Se requiere confirmación explícita de riesgo alto."
                    );
                }
            }
        }

        var loan = new Loan
        {
            LoanNumber = loanNumber,
            UserId = request.UserId,
            DisbursedAmount = request.DisbursedAmount,
            AnnualInterestRate = request.AnnualInterestRate,
            TermInMonths = request.TermInMonths,
            CurrentDebt = request.DisbursedAmount,
            Status = LoanStatus.Active,
        };

        foreach (var installment in installments)
        {
            loan.Installments.Add(installment);
        }

        await _loanRepository.AddAsync(loan);

        // Desembolso automático: acreditar monto a la cuenta principal del cliente
        await _mediator.Send(new DesembolsarPrestamoCommand { LoanId = loan.Id }, cancellationToken);

        return loan.Id;
    }
}
