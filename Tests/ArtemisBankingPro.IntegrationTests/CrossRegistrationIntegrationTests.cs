using System;
using System.Threading;
using System.Threading.Tasks;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.ExpressTransaction;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using ArtemisBankingPro.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ArtemisBankingPro.IntegrationTests;

public class CrossRegistrationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CrossRegistrationIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ExpressTransaction_ShouldPersist_CrossRegistrationAtomically()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ArtemisBankingDbContext>();

        // Clean up previous runs from in-memory DB if needed
        dbContext.SavingsAccounts.RemoveRange(dbContext.SavingsAccounts);
        dbContext.Transactions.RemoveRange(dbContext.Transactions);
        await dbContext.SaveChangesAsync();

        // SavingsAccount.UserId is int
        var sourceAccount = new SavingsAccount { AccountNumber = "100000001", Balance = 5000, UserId = 1, Status = AccountStatus.Active };
        var destAccount = new SavingsAccount { AccountNumber = "100000002", Balance = 1000, UserId = 2, Status = AccountStatus.Active };

        dbContext.SavingsAccounts.AddRange(sourceAccount, destAccount);
        await dbContext.SaveChangesAsync();

        var savingsRepo = new GenericRepository<SavingsAccount>(dbContext);
        var transactionRepo = new GenericRepository<Transaction>(dbContext);
        var userRepo = new GenericRepository<User>(dbContext);
        // InMemoryDatabase doesn't support transactions, but EF Core still executes SaveChanges.
        var uow = new UnitOfWork(dbContext);

        // Mock IEmailService – handler requires it but it's a notification side-effect
        var emailServiceMock = new Mock<IEmailService>();
        emailServiceMock
            .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var handler = new ExpressTransactionCommandHandler(savingsRepo, transactionRepo, uow, emailServiceMock.Object, userRepo);
        var command = new ExpressTransactionCommand
        {
            SourceAccountNumber = "100000001",
            DestinationAccountNumber = "100000002",
            Amount = 1500,
            RequestingUserId = 1 // matches sourceAccount.UserId
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedSource = await dbContext.SavingsAccounts.FirstOrDefaultAsync(a => a.AccountNumber == "100000001");
        var savedDest = await dbContext.SavingsAccounts.FirstOrDefaultAsync(a => a.AccountNumber == "100000002");
        var savedTransactions = await dbContext.Transactions.ToListAsync();

        savedSource.Should().NotBeNull();
        savedSource!.Balance.Should().Be(3500);

        savedDest.Should().NotBeNull();
        savedDest!.Balance.Should().Be(2500);

        savedTransactions.Should().HaveCount(2); // Cross Registration!

        savedTransactions.Should().ContainSingle(t => t.SavingsAccountId == savedSource.Id && t.Type == TransactionType.Debit && t.Amount == 1500);
        savedTransactions.Should().ContainSingle(t => t.SavingsAccountId == savedDest.Id && t.Type == TransactionType.Credit && t.Amount == 1500);
    }
}
