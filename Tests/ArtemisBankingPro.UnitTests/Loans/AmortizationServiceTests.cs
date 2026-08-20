using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using Xunit;

namespace ArtemisBankingPro.UnitTests.Loans;

public class AmortizationServiceTests
{
    private readonly AmortizationService _amortizationService;

    public AmortizationServiceTests()
    {
        _amortizationService = new AmortizationService();
    }

    [Fact]
    public void GenerateAmortizationTable_ShouldReturnCorrectNumberOfInstallments()
    {
        // Arrange
        decimal disbursedAmount = 10000m;
        decimal annualInterestRate = 12m;
        int termInMonths = 12;

        // Act
        var result = _amortizationService.GenerateAmortizationTable(disbursedAmount, annualInterestRate, termInMonths);

        // Assert
        Assert.Equal(termInMonths, result.Count);
        for (int i = 0; i < termInMonths; i++)
        {
            Assert.Equal(i + 1, result[i].InstallmentNumber);
            Assert.Equal(InstallmentStatus.Pending, result[i].Status);
        }
    }

    [Fact]
    public void GenerateAmortizationTable_ShouldWorkCorrectly_WithZeroInterestRate()
    {
        // Arrange
        decimal disbursedAmount = 1200m;
        decimal annualInterestRate = 0m;
        int termInMonths = 12;

        // Act
        var result = _amortizationService.GenerateAmortizationTable(disbursedAmount, annualInterestRate, termInMonths);

        // Assert
        Assert.Equal(termInMonths, result.Count);
        foreach (var installment in result)
        {
            Assert.Equal(100m, installment.InstallmentAmount);
            Assert.Equal(0m, installment.Interest);
            Assert.Equal(100m, installment.PrincipalAmortization);
        }
        Assert.Equal(0m, result[^1].PendingBalance);
    }

    [Fact]
    public void GenerateAmortizationTable_ShouldCalculateCorrectly_WithFrenchSystem()
    {
        // Arrange
        decimal disbursedAmount = 1000m;
        decimal annualInterestRate = 12m; // 1% mensual
        int termInMonths = 3;

        // Formula francesa cuota fija: Cuota = P * (r * (1 + r)^n) / ((1 + r)^n - 1)
        // Cuota = 1000 * (0.01 * (1.01)^3) / ((1.01)^3 - 1) = 340.02
        // Cuota redondeada = 340.02

        // Act
        var result = _amortizationService.GenerateAmortizationTable(disbursedAmount, annualInterestRate, termInMonths);

        // Assert
        Assert.Equal(3, result.Count);
        
        // Mes 1: 
        // Interés = 1000 * 0.01 = 10.00
        // Capital = 340.02 - 10.00 = 330.02
        // Balance = 1000 - 330.02 = 669.98
        Assert.Equal(10m, result[0].Interest);
        Assert.Equal(330.02m, result[0].PrincipalAmortization);
        Assert.Equal(669.98m, result[0].PendingBalance);

        // Mes 2: 
        // Interés = 669.98 * 0.01 = 6.70 (redondeado)
        // Capital = 340.02 - 6.70 = 333.32
        // Balance = 669.98 - 333.32 = 336.66
        Assert.Equal(6.70m, result[1].Interest);
        Assert.Equal(333.32m, result[1].PrincipalAmortization);
        Assert.Equal(336.66m, result[1].PendingBalance);

        // Mes 3 (último mes):
        // Interés = 336.66 * 0.01 = 3.37 (redondeado)
        // Capital = Balance restante = 336.66
        // Cuota = Capital + Interés = 336.66 + 3.37 = 340.03 (ajuste de centavo por redondeo)
        // Balance = 0.00
        Assert.Equal(3.37m, result[2].Interest);
        Assert.Equal(336.66m, result[2].PrincipalAmortization);
        Assert.Equal(340.03m, result[2].InstallmentAmount);
        Assert.Equal(0m, result[2].PendingBalance);
    }
}
