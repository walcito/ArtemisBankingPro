using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Common.Enums.Users;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using ArtemisBankingPro.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ArtemisBankingPro.IntegrationTests.Loans;

public class PrestamosIntegrationTests
{
    private ArtemisBankingDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ArtemisBankingDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;

        var context = new ArtemisBankingDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddLoanWithAmortizationTable_ShouldPersistAndRetrieveCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var loanRepository = new GenericRepository<Loan>(context);

        // Crear usuario para cumplir con FK (si existe relación)
        var user = new User
        {
            Id = 1,
            FirstName = "Juan",
            LastName = "Pérez",
            IdentificationNumber = "40200000001",
            Email = "juan@example.com",
            Username = "juanp",
            Role = Roles.Client,
            IsActive = true
        };
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        // Crear préstamo con cuotas
        var loan = new Loan
        {
            LoanNumber = "123456789",
            UserId = user.Id,
            DisbursedAmount = 1000m,
            AnnualInterestRate = 12m,
            TermInMonths = 2,
            CurrentDebt = 1000m,
            Status = LoanStatus.Active
        };

        var inst1 = new AmortizationInstallment
        {
            InstallmentNumber = 1,
            InstallmentAmount = 507.51m,
            Interest = 10.00m,
            PrincipalAmortization = 497.51m,
            PendingBalance = 502.49m,
            Status = InstallmentStatus.Pending,
            PaidAmount = 0m
        };

        var inst2 = new AmortizationInstallment
        {
            InstallmentNumber = 2,
            InstallmentAmount = 507.51m,
            Interest = 5.02m,
            PrincipalAmortization = 502.49m,
            PendingBalance = 0m,
            Status = InstallmentStatus.Pending,
            PaidAmount = 0m
        };

        loan.Installments.Add(inst1);
        loan.Installments.Add(inst2);

        // Act - Guardar en base de datos
        await loanRepository.AddAsync(loan);

        // Limpiar el rastreador de cambios de EF Core para asegurar que la consulta vaya a la base de datos
        context.ChangeTracker.Clear();

        // Act - Recuperar
        var retrievedLoans = await loanRepository.GetAllListWithIncludeAsync(l => l.Installments, l => l.User!);
        var retrievedLoan = retrievedLoans.FirstOrDefault(l => l.LoanNumber == "123456789");

        // Assert
        Assert.NotNull(retrievedLoan);
        Assert.Equal(user.Id, retrievedLoan.UserId);
        Assert.NotNull(retrievedLoan.User);
        Assert.Equal("Juan", retrievedLoan.User.FirstName);
        Assert.Equal(2, retrievedLoan.Installments.Count);

        var retrievedInstallments = retrievedLoan.Installments.OrderBy(i => i.InstallmentNumber).ToList();
        Assert.Equal(1, retrievedInstallments[0].InstallmentNumber);
        Assert.Equal(507.51m, retrievedInstallments[0].InstallmentAmount);
        Assert.Equal(10.00m, retrievedInstallments[0].Interest);
        Assert.Equal(497.51m, retrievedInstallments[0].PrincipalAmortization);
        Assert.Equal(502.49m, retrievedInstallments[0].PendingBalance);

        Assert.Equal(2, retrievedInstallments[1].InstallmentNumber);
        Assert.Equal(0m, retrievedInstallments[1].PendingBalance);
    }

    [Fact]
    public async Task UpdateInstallmentStatus_ShouldPersistChanges()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var loanRepository = new GenericRepository<Loan>(context);

        var user = new User
        {
            Id = 2,
            FirstName = "Maria",
            LastName = "Gomez",
            IdentificationNumber = "40200000002",
            Email = "maria@example.com",
            Username = "mariag",
            Role = Roles.Client,
            IsActive = true
        };
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        var loan = new Loan
        {
            LoanNumber = "987654321",
            UserId = user.Id,
            DisbursedAmount = 500m,
            AnnualInterestRate = 0m,
            TermInMonths = 1,
            CurrentDebt = 500m,
            Status = LoanStatus.Active
        };

        var installment = new AmortizationInstallment
        {
            InstallmentNumber = 1,
            InstallmentAmount = 500m,
            Interest = 0m,
            PrincipalAmortization = 500m,
            PendingBalance = 0m,
            Status = InstallmentStatus.Pending,
            PaidAmount = 0m
        };
        loan.Installments.Add(installment);

        await loanRepository.AddAsync(loan);
        context.ChangeTracker.Clear();

        // Act - Modificar estado de cuota
        var loansList = await loanRepository.GetAllListWithIncludeAsync(l => l.Installments);
        var dbLoan = loansList.First(l => l.LoanNumber == "987654321");
        var dbInstallment = dbLoan.Installments.First();

        dbInstallment.Status = InstallmentStatus.Paid;
        dbInstallment.PaidAmount = 500m;
        dbLoan.CurrentDebt = 0m;
        dbLoan.Status = LoanStatus.Paid;

        await loanRepository.UpdateAsync(dbLoan);
        context.ChangeTracker.Clear();

        // Assert - Recuperar y verificar actualización en la base de datos
        var verifiedLoans = await loanRepository.GetAllListWithIncludeAsync(l => l.Installments);
        var verifiedLoan = verifiedLoans.First(l => l.LoanNumber == "987654321");
        var verifiedInstallment = verifiedLoan.Installments.First();

        Assert.Equal(LoanStatus.Paid, verifiedLoan.Status);
        Assert.Equal(0m, verifiedLoan.CurrentDebt);
        Assert.Equal(InstallmentStatus.Paid, verifiedInstallment.Status);
        Assert.Equal(500m, verifiedInstallment.PaidAmount);
    }
}
