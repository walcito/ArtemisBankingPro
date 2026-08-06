using ArtemisBankingPro.Core.Application.Features.Trades.Commands.UpdateTrade;
using ArtemisBankingPro.Core.Application.Interfaces.Repositories;
using ArtemisBankingPro.Core.Domain.Entities;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.Trades;

public class UpdateTradeCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenMailBelongsToAnotherTrade()
    {
        // Arrange
        var tradeToUpdate = new Trade { Id = 1, Name = "Comercio A", Mail = "a@correo.com", Rnc = "111" };
        var otherTrade = new Trade { Id = 2, Name = "Comercio B", Mail = "b@correo.com", Rnc = "222" };

        var tradeRepoMock = new Mock<IGenericRepository<Trade>>();
        tradeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tradeToUpdate);
        tradeRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Trade> { tradeToUpdate, otherTrade });

        var handler = new UpdateTradeCommandHandler(tradeRepoMock.Object);

        var command = new UpdateTradeCommand
        {
            Id = 1,
            Name = "Comercio A Actualizado",
            Mail = "b@correo.com", // intenta usar el correo del Comercio B
            Phone = "8091234567"
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenKeepingOwnMailUnchanged()
    {
        // Arrange
        var tradeToUpdate = new Trade { Id = 1, Name = "Comercio A", Mail = "a@correo.com", Rnc = "111" };

        var tradeRepoMock = new Mock<IGenericRepository<Trade>>();
        tradeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tradeToUpdate);
        tradeRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Trade> { tradeToUpdate });

        var handler = new UpdateTradeCommandHandler(tradeRepoMock.Object);

        var command = new UpdateTradeCommand
        {
            Id = 1,
            Name = "Comercio A Actualizado",
            Mail = "a@correo.com", // se queda con su propio correo
            Phone = "8091234567"
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Comercio A Actualizado", tradeToUpdate.Name);
    }
}