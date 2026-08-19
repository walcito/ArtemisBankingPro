using ArtemisBankingPro.Core.Application.ViewModels.User;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ArtemisBankingPro.UnitTests;

public class ValidationTests
{
    [Fact]
    public void SaveUserViewModel_ShouldHaveValidationError_WhenRequiredFieldsAreEmpty()
    {
        // Arrange
        var vm = new SaveUserViewModel
        {
            FirstName = "",
            LastName = "",
            Email = "invalid-email",
            UserName = ""
        };

        var context = new ValidationContext(vm);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(vm, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.True(results.Count >= 3);
    }

    [Fact]
    public void SaveUserViewModel_ShouldBeValid_WhenAllFieldsAreCorrect()
    {
        // Arrange
        var vm = new SaveUserViewModel
        {
            FirstName = "Maria",
            LastName = "Gomez",
            Identification = "402-1234567-8",
            Email = "maria@ejemplo.com",
            UserName = "mgomez",
            Password = "123P@$$word!",
            ConfirmPassword = "123P@$$word!",
            Role = "Admin"
        };

        var context = new ValidationContext(vm);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(vm, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}
