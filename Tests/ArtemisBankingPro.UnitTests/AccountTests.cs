using ArtemisBankingPro.Core.Application.Dtos.Account;
using ArtemisBankingPro.Core.Application.Features.Account;
using ArtemisBankingPro.Core.Application.Interfaces;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests;

public class AccountTests
{
    private readonly Mock<IAccountService> _accountServiceMock;

    public AccountTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
    }

    [Fact]
    public async Task AuthenticateCommand_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new AuthRequest { UserName = "admin", Password = "123P@$$word!" };
        var expectedResponse = new AuthResponse { Id = "123", UserName = "admin", Role = "Admin", HasError = false };

        _accountServiceMock.Setup(s => s.AuthenticateAsync(It.IsAny<AuthRequest>()))
            .ReturnsAsync(expectedResponse);

        var handler = new AuthenticateCommandHandler(_accountServiceMock.Object);
        var command = new AuthenticateCommand { Request = request };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasError);
        Assert.Equal("admin", result.UserName);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task ConfirmAccountCommand_ShouldReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var request = new ConfirmAccountRequest { UserId = "user-123", Token = "valid-token" };

        _accountServiceMock.Setup(s => s.ConfirmAccountAsync("user-123", "valid-token"))
            .ReturnsAsync(true);

        var handler = new ConfirmAccountCommandHandler(_accountServiceMock.Object);
        var command = new ConfirmAccountCommand { Request = request };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}
