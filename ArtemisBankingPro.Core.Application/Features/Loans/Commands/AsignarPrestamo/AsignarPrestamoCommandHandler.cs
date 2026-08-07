using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Enums;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.AsignarPrestamo;

public class AsignarPrestamoCommandHandler : IRequestHandler<AsignarPrestamoCommand, int>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IAmortizationService _amortizationService;
    private readonly GeneradorNumeroPrestamoService _generadorNumeroPrestamo;

    public AsignarPrestamoCommandHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<Loan> loanRepository,
        IAmortizationService amortizationService,
        GeneradorNumeroPrestamoService generadorNumeroPrestamo)
    {
        _userRepository = userRepository;
        _loanRepository = loanRepository;
        _amortizationService = amortizationService;
        _generadorNumeroPrestamo = generadorNumeroPrestamo;
    }

    public async Task<int> Handle(AsignarPrestamoCommand request, CancellationToken cancellationToken)
    {
        // Validar que el usuario existe y está activo
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null || !user.IsActive)
            throw new Exception("El cliente no existe o no está activo.");

        // Validar que no tenga un préstamo activo
        var loans = await _loanRepository.GetAllAsync();
        var hasActiveLoan = loans.Any(l => l.UserId == request.UserId && l.Status == LoanStatus.Active);

        if (hasActiveLoan)
            throw new Exception("El cliente ya tiene un préstamo activo.");

        // Generar número de préstamo único
        var loanNumber = await _generadorNumeroPrestamo.GenerarNumeroUnicoAsync();

        // Generar tabla de amortización
        var installments = _amortizationService.GenerateAmortizationTable(
            request.DisbursedAmount,
            request.AnnualInterestRate,
            request.TermInMonths
        );

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

        // Asociar cuotas al préstamo
        foreach (var installment in installments)
        {
            loan.Installments.Add(installment);
        }

        await _loanRepository.AddAsync(loan);

        return loan.Id;
    }
}
