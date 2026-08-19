using System.Threading;
using System.Threading.Tasks;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.AddBeneficiary;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using ArtemisBankingPro.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ArtemisBankingPro.IntegrationTests;

public class BeneficiaryAndSavingsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BeneficiaryAndSavingsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddBeneficiary_ShouldPersist_ToDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ArtemisBankingDbContext>();
        
        var beneficiaryRepo = new GenericRepository<Beneficiary>(dbContext);
        var savingsRepo = new GenericRepository<SavingsAccount>(dbContext);

        // Seed destination account
        var destAccount = new SavingsAccount { AccountNumber = "555666777", UserId = "other-user", Status = AccountStatus.Active };
        dbContext.SavingsAccounts.Add(destAccount);
        await dbContext.SaveChangesAsync();

        var handler = new AddBeneficiaryCommandHandler(beneficiaryRepo, savingsRepo);
        var command = new AddBeneficiaryCommand 
        { 
            RequestingUserId = "my-user", 
            DestinationAccountNumber = "555666777" 
        };

        // Act
        var newBeneficiaryId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedBeneficiary = await dbContext.Beneficiaries.FindAsync(newBeneficiaryId);
        savedBeneficiary.Should().NotBeNull();
        savedBeneficiary!.UserId.Should().Be("my-user");
        savedBeneficiary.DestinationAccountNumber.Should().Be("555666777");
        savedBeneficiary.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSavingsAccount_ShouldPersist_WithAllMandatoryFields()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ArtemisBankingDbContext>();
        
        var savingsRepo = new GenericRepository<SavingsAccount>(dbContext);
        var transactionRepo = new GenericRepository<Transaction>(dbContext);
        var userRepo = new GenericRepository<User>(dbContext);

        // Need to add user and a primary account so we can create a secondary account
        var userId = "test-user-savings";
        dbContext.Users.Add(new User { Id = userId, IsActive = true });
        dbContext.SavingsAccounts.Add(new SavingsAccount { AccountNumber = "000000000", Type = AccountType.Principal, Status = AccountStatus.Active, UserId = userId });
        await dbContext.SaveChangesAsync();

        var handler = new CreateSavingsAccountCommandHandler(userRepo, savingsRepo, transactionRepo);
        var command = new CreateSavingsAccountCommand
        {
            UserId = userId,
            InitialAmount = 2500
        };

        // Act
        var newAccountId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedAccount = await dbContext.SavingsAccounts.FindAsync(newAccountId);
        savedAccount.Should().NotBeNull();
        savedAccount!.AccountNumber.Should().NotBeNullOrEmpty();
        savedAccount.AccountNumber.Should().HaveLength(9);
        savedAccount.Balance.Should().Be(2500);
        savedAccount.Type.Should().Be(AccountType.Secondary);
        savedAccount.Status.Should().Be(AccountStatus.Active);
        savedAccount.UserId.Should().Be(userId);

        var creditTransaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.SavingsAccountId == newAccountId);
        creditTransaction.Should().NotBeNull();
        creditTransaction!.Amount.Should().Be(2500);
        creditTransaction.Type.Should().Be(TransactionType.Credit);
    }
}
