using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.Commands;

public class SavingsAccountCommandsTests
{
    private readonly Mock<IGenericRepository<SavingsAccount>> _mockAccountRepo;
    private readonly Mock<IGenericRepository<Transaction>> _mockTransactionRepo;
    private readonly Mock<IGenericRepository<User>> _mockUserRepo;
    private readonly Mock<IUnitOfWork> _mockUow;

    public SavingsAccountCommandsTests()
    {
        _mockAccountRepo = new Mock<IGenericRepository<SavingsAccount>>();
        _mockTransactionRepo = new Mock<IGenericRepository<Transaction>>();
        _mockUserRepo = new Mock<IGenericRepository<User>>();
        _mockUow = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task CreateSecondaryAccount_WithoutActivePrimary_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        _mockUserRepo.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, IsActive = true });

        // No primary accounts exist for this user
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount>());

        var handler = new CreateSavingsAccountCommandHandler(_mockUserRepo.Object, _mockAccountRepo.Object, _mockTransactionRepo.Object);
        var command = new CreateSavingsAccountCommand { UserId = userId, InitialAmount = 100 };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("El cliente debe tener una cuenta principal activa antes de crear una cuenta secundaria.");
    }

    [Fact]
    public async Task CancelSecondaryAccount_WithNoPrincipalFound_ShouldThrowException()
    {
        // Arrange
        var secondaryAccount = new SavingsAccount { Id = 2, AccountNumber = "222", UserId = 1, Type = AccountType.Secondary, Status = AccountStatus.Active };

        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { secondaryAccount });

        var handler = new CancelSavingsAccountCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object);
        var command = new CancelSavingsAccountCommand { AccountNumber = "222" };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("No se encontró una cuenta principal activa para transferir el balance.");
    }
}
