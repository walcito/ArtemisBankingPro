using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using ArtemisBankingPro.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ArtemisBankingPro.IntegrationTests.Loans;

public class DataSeederTests
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
    public async Task SeedAsync_ShouldPopulateDatabaseCorrectly_WhenDatabaseIsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        // 1. Verificar Usuarios
        var users = await context.Set<User>().ToListAsync();
        Assert.Equal(3, users.Count);
        
        var juan = users.FirstOrDefault(u => u.Username == "juanp");
        var maria = users.FirstOrDefault(u => u.Username == "mariar");
        var pedro = users.FirstOrDefault(u => u.Username == "pedrom");

        Assert.NotNull(juan);
        Assert.NotNull(maria);
        Assert.NotNull(pedro);

        Assert.Equal("Juan", juan.FirstName);
        Assert.Equal("María", maria.FirstName);
        Assert.Equal("Pedro", pedro.FirstName);

        Assert.All(users, u => Assert.Equal(Roles.Client, u.Role));
        Assert.All(users, u => Assert.True(u.IsActive));

        // 2. Verificar Cuentas de Ahorro
        var accounts = await context.Set<SavingsAccount>().ToListAsync();
        Assert.Equal(3, accounts.Count);

        var accJuan = accounts.FirstOrDefault(a => a.UserId == juan.Id);
        var accMaria = accounts.FirstOrDefault(a => a.UserId == maria.Id);
        var accPedro = accounts.FirstOrDefault(a => a.UserId == pedro.Id);

        Assert.NotNull(accJuan);
        Assert.NotNull(accMaria);
        Assert.NotNull(accPedro);

        Assert.Equal("100000001", accJuan.AccountNumber);
        Assert.Equal(150000.00m, accJuan.Balance);
        Assert.Equal(AccountType.Principal, accJuan.Type);
        Assert.Equal(AccountStatus.Active, accJuan.Status);

        Assert.Equal("100000002", accMaria.AccountNumber);
        Assert.Equal(250000.00m, accMaria.Balance);

        Assert.Equal("100000003", accPedro.AccountNumber);
        Assert.Equal(5000.00m, accPedro.Balance);

        // 3. Verificar Préstamos
        var loans = await context.Loans.Include(l => l.Installments).ToListAsync();
        Assert.Equal(3, loans.Count);

        var loanJuan = loans.FirstOrDefault(l => l.UserId == juan.Id);
        var loanMaria = loans.FirstOrDefault(l => l.UserId == maria.Id);
        var loanPedro = loans.FirstOrDefault(l => l.UserId == pedro.Id);

        Assert.NotNull(loanJuan);
        Assert.NotNull(loanMaria);
        Assert.NotNull(loanPedro);

        // Préstamo 1 (Juan)
        Assert.Equal("900000001", loanJuan.LoanNumber);
        Assert.Equal(120000.00m, loanJuan.DisbursedAmount);
        Assert.Equal(18.0m, loanJuan.AnnualInterestRate);
        Assert.Equal(12, loanJuan.TermInMonths);
        Assert.Equal(LoanStatus.Active, loanJuan.Status);
        Assert.Equal(12, loanJuan.Installments.Count);

        var installmentsJuan = loanJuan.Installments.OrderBy(i => i.InstallmentNumber).ToList();
        // Primeras 4 cuotas deben estar pagadas
        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(InstallmentStatus.Paid, installmentsJuan[i].Status);
            Assert.Equal(installmentsJuan[i].InstallmentAmount, installmentsJuan[i].PaidAmount);
        }
        // 5ta cuota parcialmente pagada con 3000
        Assert.Equal(InstallmentStatus.PartiallyPaid, installmentsJuan[4].Status);
        Assert.Equal(3000.00m, installmentsJuan[4].PaidAmount);
        // Resto deben ser pendientes
        for (int i = 5; i < 12; i++)
        {
            Assert.Equal(InstallmentStatus.Pending, installmentsJuan[i].Status);
            Assert.Equal(0m, installmentsJuan[i].PaidAmount);
        }
        // Deuda actual menor que el desembolso y positiva
        Assert.True(loanJuan.CurrentDebt < loanJuan.DisbursedAmount);
        Assert.True(loanJuan.CurrentDebt > 0m);

        // Préstamo 2 (María)
        Assert.Equal("900000002", loanMaria.LoanNumber);
        Assert.Equal(50000.00m, loanMaria.DisbursedAmount);
        Assert.Equal(15.0m, loanMaria.AnnualInterestRate);
        Assert.Equal(6, loanMaria.TermInMonths);
        Assert.Equal(LoanStatus.Paid, loanMaria.Status);
        Assert.Equal(0.00m, loanMaria.CurrentDebt);
        Assert.Equal(6, loanMaria.Installments.Count);
        Assert.All(loanMaria.Installments, inst => Assert.Equal(InstallmentStatus.Paid, inst.Status));

        // Préstamo 3 (Pedro)
        Assert.Equal("900000003", loanPedro.LoanNumber);
        Assert.Equal(200000.00m, loanPedro.DisbursedAmount);
        Assert.Equal(20.0m, loanPedro.AnnualInterestRate);
        Assert.Equal(24, loanPedro.TermInMonths);
        Assert.Equal(LoanStatus.Active, loanPedro.Status);
        Assert.Equal(200000.00m, loanPedro.CurrentDebt);
        Assert.Equal(24, loanPedro.Installments.Count);
        Assert.All(loanPedro.Installments, inst => Assert.Equal(InstallmentStatus.Pending, inst.Status));
        Assert.All(loanPedro.Installments, inst => Assert.Equal(0m, inst.PaidAmount));
    }

    [Fact]
    public async Task SeedAsync_ShouldDoNothing_WhenDatabaseIsNotEmpty()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var dummyUser = new User
        {
            FirstName = "Dummy",
            LastName = "User",
            IdentificationNumber = "00000000000",
            Email = "dummy@example.com",
            Username = "dummy",
            Role = Roles.Client,
            IsActive = true
        };
        context.Set<User>().Add(dummyUser);
        await context.SaveChangesAsync();

        // Act
        await DataSeeder.SeedAsync(context);

        // Assert
        var users = await context.Set<User>().ToListAsync();
        Assert.Single(users);
        Assert.Equal("dummy", users[0].Username);

        var loans = await context.Loans.ToListAsync();
        Assert.Empty(loans);
    }
}
