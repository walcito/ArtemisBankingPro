using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Enums;
using ArtemisBankingPro.Core.Domain.Interfaces;

namespace ArtemisBankingPro.Core.Application.Services;

// Implementación del sistema de amortización francés (cuota fija)
public class CalculadoraAmortizationService : IAmortizationService
{
    public List<AmortizationInstallment> GenerateAmortizationTable(decimal disbursedAmount, decimal annualInterestRate, int termInMonths)
    {
        var installments = new List<AmortizationInstallment>();

        // Tasa mensual (ej: 12% anual → 0.01 mensual)
        decimal monthlyRate = annualInterestRate / 12m / 100m;

        // Cuota fija: P × [ r × (1 + r)^n ] / [ (1 + r)^n − 1 ]
        decimal fixedInstallment;

        if (monthlyRate == 0)
        {
            fixedInstallment = disbursedAmount / termInMonths;
        }
        else
        {
            double rateDouble = (double)monthlyRate;
            double factor = Math.Pow(1 + rateDouble, termInMonths);
            fixedInstallment = disbursedAmount * (decimal)(rateDouble * factor / (factor - 1));
        }

        fixedInstallment = Math.Round(fixedInstallment, 2);
        decimal pendingBalance = disbursedAmount;

        for (int i = 1; i <= termInMonths; i++)
        {
            decimal interest = Math.Round(pendingBalance * monthlyRate, 2);
            decimal principalAmortization = fixedInstallment - interest;

            // Última cuota: ajustar para eliminar diferencias de redondeo
            if (i == termInMonths)
            {
                principalAmortization = pendingBalance;
                fixedInstallment = principalAmortization + interest;
            }

            pendingBalance -= principalAmortization;

            var installment = new AmortizationInstallment
            {
                InstallmentNumber = i,
                InstallmentAmount = fixedInstallment,
                Interest = interest,
                PrincipalAmortization = principalAmortization,
                PendingBalance = Math.Max(pendingBalance, 0),
                Status = InstallmentStatus.Pending,
                PaidAmount = 0
            };

            installments.Add(installment);
        }

        return installments;
    }
}
