using ArtemisBankingPro.Core.Domain.Entities.Loans;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface IAmortizationService
{
    // Genera la tabla de amortización completa (sistema francés, cuota fija)
    List<AmortizationInstallment> GenerateAmortizationTable(decimal disbursedAmount, decimal annualInterestRate, int termInMonths);
}
