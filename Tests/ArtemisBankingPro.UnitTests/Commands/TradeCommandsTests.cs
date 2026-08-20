using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArtemisBankingPro.Core.Application.Features.Trades.Commands.CreateTrade;
using ArtemisBankingPro.Core.Application.Features.Trades.Commands.ToggleTradeStatus;
using ArtemisBankingPro.Core.Domain.Entities.Trades;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.Commands;

public class TradeCommandsTests
{
    private readonly Mock<IGenericRepository<Trade>> _mockTradeRepo;
    private readonly Mock<IGenericRepository<User>> _mockUserRepo;

    public TradeCommandsTests()
    {
        _mockTradeRepo = new Mock<IGenericRepository<Trade>>();
        _mockUserRepo = new Mock<IGenericRepository<User>>();
    }

    [Fact]
    public async Task CreateTrade_WithExistingRnc_ShouldThrowException()
    {
        // Arrange
        var existingTrades = new List<Trade>
        {
            new Trade { Rnc = "123456789", Mail = "other@mail.com" }
        };
        _mockTradeRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(existingTrades);

        var handler = new CreateTradeCommandHandler(_mockTradeRepo.Object);
        var command = new CreateTradeCommand { Rnc = "123456789", Mail = "new@mail.com" };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Ya existe un comercio registrado con este RNC.");
    }

    [Fact]
    public async Task CreateTrade_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var existingTrades = new List<Trade>
        {
            new Trade { Rnc = "000000000", Mail = "test@mail.com" }
        };
        _mockTradeRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(existingTrades);

        var handler = new CreateTradeCommandHandler(_mockTradeRepo.Object);
        var command = new CreateTradeCommand { Rnc = "111111111", Mail = "test@mail.com" };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Ya existe un comercio registrado con este correo.");
    }

    [Fact]
    public async Task ToggleStatus_ToInactive_ShouldInactivateAssociatedUser()
    {
        // Arrange
        var userId = 1;
        var trade = new Trade { Id = 1, IsActive = true, AssociatedUserId = userId };
        var user = new User { Id = userId, IsActive = true };

        _mockTradeRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(trade);
        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

        var handler = new ToggleTradeStatusCommandHandler(_mockTradeRepo.Object, _mockUserRepo.Object);
        var command = new ToggleTradeStatusCommand { Id = 1 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        trade.IsActive.Should().BeFalse();
        user.IsActive.Should().BeFalse(); // Logic en cascada
        _mockTradeRepo.Verify(x => x.Update(trade), Times.Once);
        _mockUserRepo.Verify(x => x.Update(user), Times.Once);
    }

    [Fact]
    public async Task ToggleStatus_ToActive_ShouldNotReactivateAssociatedUser()
    {
        // Arrange
        var userId = 2;
        var trade = new Trade { Id = 2, IsActive = false, AssociatedUserId = userId };
        var user = new User { Id = userId, IsActive = false }; // Was inactivated previously

        _mockTradeRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(trade);
        // Even if we mock the user, the handler should not attempt to reactivate it.
        
        var handler = new ToggleTradeStatusCommandHandler(_mockTradeRepo.Object, _mockUserRepo.Object);
        var command = new ToggleTradeStatusCommand { Id = 2 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        trade.IsActive.Should().BeTrue();
        user.IsActive.Should().BeFalse(); // Does NOT reactivate
        _mockTradeRepo.Verify(x => x.Update(trade), Times.Once);
        _mockUserRepo.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
    }
}
