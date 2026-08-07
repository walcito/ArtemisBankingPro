using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Enums;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.ModificarTasa;

public class ModificarTasaCommandHandler : IRequestHandler<ModificarTasaCommand, bool>
{
    private readonly IGenericRepository<Loan> _loanRepository;

    public ModificarTasaCommandHandler(IGenericRepository<Loan> loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<bool> Handle(ModificarTasaCommand request, CancellationToken cancellationToken)
    {
        // Obtener el préstamo con sus cuotas
        var loans = await _loanRepository.GetAllListWithIncludeAsync(l => l.Installments);
        var loan = loans.FirstOrDefault(l => l.Id == request.LoanId);

        if (loan is null)
            throw new Exception("El préstamo no existe.");

        if (loan.Status != LoanStatus.Active)
            throw new Exception("Solo se puede modificar la tasa de interés de préstamos activos.");

        // Obtener las cuotas futuras pendientes (que no estén pagadas)
        var unpaidInstallments = loan.Installments
            .Where(i => i.Status != InstallmentStatus.Paid)
            .OrderBy(i => i.InstallmentNumber)
            .ToList();

        if (!unpaidInstallments.Any())
            return false;

        // El saldo pendiente actual es la suma del capital pendiente por amortizar de todas las cuotas no pagadas
        decimal pendingBalance = unpaidInstallments.Sum(i => i.PrincipalAmortization);
        int termRemaining = unpaidInstallments.Count;

        // Calcular nueva tasa mensual
        decimal monthlyRate = request.NewAnnualInterestRate / 12m / 100m;
        decimal newFixedInstallment;

        if (monthlyRate == 0)
        {
            newFixedInstallment = pendingBalance / termRemaining;
        }
        else
        {
            double rateDouble = (double)monthlyRate;
            double factor = Math.Pow(1 + rateDouble, termRemaining);
            newFixedInstallment = pendingBalance * (decimal)(rateDouble * factor / (factor - 1));
        }

        newFixedInstallment = Math.Round(newFixedInstallment, 2);
        decimal currentBalance = pendingBalance;

        // Recalcular cada una de las cuotas pendientes
        for (int i = 0; i < termRemaining; i++)
        {
            var installment = unpaidInstallments[i];
            decimal interest = Math.Round(currentBalance * monthlyRate, 2);
            decimal principalAmortization = newFixedInstallment - interest;

            // En la última cuota, ajustar por redondeos
            if (i == termRemaining - 1)
            {
                principalAmortization = currentBalance;
                newFixedInstallment = principalAmortization + interest;
            }

            currentBalance -= principalAmortization;

            installment.InstallmentAmount = newFixedInstallment;
            installment.Interest = interest;
            installment.PrincipalAmortization = principalAmortization;
            installment.PendingBalance = Math.Max(currentBalance, 0);
        }

        // Actualizar la tasa del préstamo
        loan.AnnualInterestRate = request.NewAnnualInterestRate;

        await _loanRepository.UpdateAsync(loan);

        return true;
    }
}
