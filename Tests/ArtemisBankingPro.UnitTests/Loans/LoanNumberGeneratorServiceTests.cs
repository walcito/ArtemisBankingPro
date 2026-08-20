using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.Loans;

public class LoanNumberGeneratorServiceTests
{
    private readonly Mock<IGenericRepository<Loan>> _loanRepoMock;
    private readonly LoanNumberGeneratorService _service;

    public LoanNumberGeneratorServiceTests()
    {
        _loanRepoMock = new Mock<IGenericRepository<Loan>>();
        _service = new LoanNumberGeneratorService(_loanRepoMock.Object);
    }

    [Fact]
    public async Task GenerateUniqueLoanNumberAsync_ShouldReturnNineDigitNumber()
    {
        // Arrange
        _loanRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Loan>());

        // Act
        var loanNumber = await _service.GenerateUniqueLoanNumberAsync();

        // Assert
        Assert.NotNull(loanNumber);
        Assert.Equal(9, loanNumber.Length);
        Assert.True(long.TryParse(loanNumber, out _));
    }

    [Fact]
    public async Task GenerateUniqueLoanNumberAsync_ShouldNotReturnExistingLoanNumber()
    {
        // Arrange
        var existingLoans = new List<Loan>
        {
            new Loan { LoanNumber = "123456789" },
            new Loan { LoanNumber = "987654321" }
        };
        _loanRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(existingLoans);

        // Act
        var loanNumber = await _service.GenerateUniqueLoanNumberAsync();

        // Assert
        Assert.NotNull(loanNumber);
        Assert.Equal(9, loanNumber.Length);
        Assert.DoesNotContain(existingLoans, l => l.LoanNumber == loanNumber);
    }
}
