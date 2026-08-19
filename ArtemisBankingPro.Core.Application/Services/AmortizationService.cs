using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Application.Interfaces;

namespace ArtemisBankingPro.Core.Application.Services;

// French amortization system implementation (fixed installment)
public class AmortizationService : IAmortizationService
{
    public List<AmortizationInstallment> GenerateAmortizationTable(decimal disbursedAmount, decimal annualInterestRate, int termInMonths)
    {
        var installments = new List<AmortizationInstallment>();

        // Monthly rate (e.g., 12% annual -> 0.01 monthly)
        decimal monthlyRate = annualInterestRate / 12m / 100m;

        // Fixed installment: 
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
