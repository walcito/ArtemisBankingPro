using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.SavingsAccounts;

public class CreateSavingsAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenUserHasNoActivePrincipalAccount()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "Juan",
            LastName = "Perez",
            IsActive = true
        };

        var userRepoMock = new Mock<IGenericRepository<User>>();
        userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        // Lista vacía: el usuario NO tiene ninguna cuenta principal
        savingsRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<SavingsAccount>());

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();

        var handler = new CreateSavingsAccountCommandHandler(
            userRepoMock.Object, savingsRepoMock.Object, transactionRepoMock.Object);

        var command = new CreateSavingsAccountCommand { UserId = 1, InitialAmount = 100m };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotActive()
    {
        // Arrange
        var inactiveUser = new User
        {
            Id = 2,
            FirstName = "Maria",
            LastName = "Lopez",
            IsActive = false
        };

        var userRepoMock = new Mock<IGenericRepository<User>>();
        userRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(inactiveUser);

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();

        var handler = new CreateSavingsAccountCommandHandler(
            userRepoMock.Object, savingsRepoMock.Object, transactionRepoMock.Object);

        var command = new CreateSavingsAccountCommand { UserId = 2 };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
}