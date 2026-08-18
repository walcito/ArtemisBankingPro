using ArtemisBankingPro.Core.Application.Features.Trades.Commands.ToggleTradeStatus;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.Trades;

public class ToggleTradeStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeactivateAssociatedUser_WhenDeactivatingTrade()
    {
        // Arrange
        var associatedUser = new User { Id = 10, IsActive = true };
        var trade = new Trade { Id = 1, IsActive = true, AssociatedUserId = 10 };

        var tradeRepoMock = new Mock<IGenericRepository<Trade>>();
        tradeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(trade);

        var userRepoMock = new Mock<IGenericRepository<User>>();
        userRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(associatedUser);

        var handler = new ToggleTradeStatusCommandHandler(tradeRepoMock.Object, userRepoMock.Object);

        // Act
        await handler.Handle(new ToggleTradeStatusCommand { Id = 1 }, CancellationToken.None);

        // Assert
        Assert.False(trade.IsActive);
        Assert.False(associatedUser.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldNotReactivateAssociatedUser_WhenReactivatingTrade()
    {
        // Arrange: el comercio está inactivo, y su usuario también quedó inactivo previamente
        var associatedUser = new User { Id = 10, IsActive = false };
        var trade = new Trade { Id = 1, IsActive = false, AssociatedUserId = 10 };

        var tradeRepoMock = new Mock<IGenericRepository<Trade>>();
        tradeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(trade);

        var userRepoMock = new Mock<IGenericRepository<User>>();
        userRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(associatedUser);

        var handler = new ToggleTradeStatusCommandHandler(tradeRepoMock.Object, userRepoMock.Object);

        // Act
        await handler.Handle(new ToggleTradeStatusCommand { Id = 1 }, CancellationToken.None);

        // Assert
        Assert.True(trade.IsActive);              // el comercio SÍ se reactiva
        Assert.False(associatedUser.IsActive);     // el usuario NO se reactiva automáticamente
    }
}