using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Features.Users;
using ArtemisBankingPro.Core.Application.Interfaces;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests;

public class UserTests
{
    private readonly Mock<IUserService> _userServiceMock;

    public UserTests()
    {
        _userServiceMock = new Mock<IUserService>();
    }

    [Fact]
    public async Task CreateUserCommand_ShouldReturnCreatedUserDto()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            FirstName = "Juan",
            LastName = "Perez",
            Identification = "402-0000000-1",
            Email = "juan@ejemplo.com",
            UserName = "jperez",
            Password = "123P@$$word!",
            Role = "Client",
            InitialAmount = 1000.00m
        };

        var expectedDto = new UserDto
        {
            Id = "usr-1",
            FirstName = "Juan",
            LastName = "Perez",
            Email = "juan@ejemplo.com",
            UserName = "jperez",
            Role = "Client",
            IsActive = false
        };

        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(expectedDto);

        var handler = new CreateUserCommandHandler(_userServiceMock.Object);
        var command = new CreateUserCommand { User = createDto };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("usr-1", result.Id);
        Assert.Equal("jperez", result.UserName);
        Assert.Equal("Client", result.Role);
    }

    [Fact]
    public async Task ChangeUserStatusCommand_ShouldReturnTrue_WhenStatusUpdated()
    {
        // Arrange
        _userServiceMock.Setup(s => s.ChangeStatusAsync("usr-1", true))
            .Returns(Task.CompletedTask);

        var handler = new ChangeUserStatusCommandHandler(_userServiceMock.Object);
        var command = new ChangeUserStatusCommand { Id = "usr-1", Status = true };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _userServiceMock.Verify(s => s.ChangeStatusAsync("usr-1", true), Times.Once);
    }
}
