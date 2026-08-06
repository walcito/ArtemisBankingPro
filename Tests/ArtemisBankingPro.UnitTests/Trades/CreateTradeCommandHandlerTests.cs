using ArtemisBankingPro.Core.Application.Features.Trades.Commands.CreateTrade;
using ArtemisBankingPro.Core.Application.Interfaces.Repositories;
using ArtemisBankingPro.Core.Domain.Entities;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.Trades;

public class CreateTradeCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenRncAlreadyExists()
    {
        // Arrange
        var existingTrade = new Trade
        {
            Id = 1,
            Name = "Comercio Existente",
            Rnc = "123456789",
            Mail = "existente@correo.com"
        };

        var tradeRepoMock = new Mock<IGenericRepository<Trade>>();
        tradeRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Trade> { existingTrade });

        var handler = new CreateTradeCommandHandler(tradeRepoMock.Object);

        var command = new CreateTradeCommand
        {
            Name = "Comercio Nuevo",
            Rnc = "123456789", // mismo RNC que el existente
            Mail = "nuevo@correo.com"
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenMailAlreadyExists()
    {
        // Arrange
        var existingTrade = new Trade
        {
            Id = 1,
            Name = "Comercio Existente",
            Rnc = "111111111",
            Mail = "correo@duplicado.com"
        };

        var tradeRepoMock = new Mock<IGenericRepository<Trade>>();
        tradeRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Trade> { existingTrade });

        var handler = new CreateTradeCommandHandler(tradeRepoMock.Object);

        var command = new CreateTradeCommand
        {
            Name = "Comercio Nuevo",
            Rnc = "222222222",
            Mail = "correo@duplicado.com" // mismo correo que el existente
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
}