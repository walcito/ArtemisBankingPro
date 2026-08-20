using ArtemisBankingPro.Core.Application.Features.Loans.Commands.PagarPrestamo;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.Loans;

public class PagarPrestamoCommandTests
{
    private readonly Mock<IGenericRepository<Loan>> _loanRepoMock;
    private readonly Mock<IGenericRepository<SavingsAccount>> _savingsAccountRepoMock;
    private readonly Mock<IGenericRepository<Transaction>> _transactionRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PagarPrestamoCommandHandler _handler;

    public PagarPrestamoCommandTests()
    {
        _loanRepoMock = new Mock<IGenericRepository<Loan>>();
        _savingsAccountRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        _transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new PagarPrestamoCommandHandler(
            _loanRepoMock.Object,
            _savingsAccountRepoMock.Object,
            _transactionRepoMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenLoanDoesNotExist()
    {
        // Arrange
        var command = new PagarPrestamoCommand { LoanId = 999, Amount = 100, SavingsAccountId = 1 };
        _loanRepoMock.Setup(r => r.GetAllListWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Loan, object>>[]>()))
            .ReturnsAsync(new List<Loan>());

        // Act & Assert
        var ex = await Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("El préstamo no existe.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenLoanIsNotActive()
    {
        // Arrange
        var command = new PagarPrestamoCommand { LoanId = 1, Amount = 100, SavingsAccountId = 1 };
        var loan = new Loan { Id = 1, Status = LoanStatus.Paid };
        _loanRepoMock.Setup(r => r.GetAllListWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Loan, object>>[]>()))
            .ReturnsAsync(new List<Loan> { loan });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("El préstamo no está activo.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSavingsAccountDoesNotExist()
    {
        // Arrange
        var command = new PagarPrestamoCommand { LoanId = 1, Amount = 100, SavingsAccountId = 999 };
        var loan = new Loan { Id = 1, Status = LoanStatus.Active };
        _loanRepoMock.Setup(r => r.GetAllListWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Loan, object>>[]>()))
            .ReturnsAsync(new List<Loan> { loan });
        _savingsAccountRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((SavingsAccount?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("La cuenta de ahorros seleccionada no existe.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenSavingsAccountIsNotActive()
    {
        // Arrange
        var command = new PagarPrestamoCommand { LoanId = 1, Amount = 100, SavingsAccountId = 1 };
        var loan = new Loan { Id = 1, Status = LoanStatus.Active };
        var account = new SavingsAccount { Id = 1, Status = AccountStatus.Cancelled, Balance = 500 };
        
        _loanRepoMock.Setup(r => r.GetAllListWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Loan, object>>[]>()))
            .ReturnsAsync(new List<Loan> { loan });
        _savingsAccountRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("La cuenta de ahorros seleccionada no está activa.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenBalanceIsInsufficient()
    {
        // Arrange
        var command = new PagarPrestamoCommand { LoanId = 1, Amount = 100, SavingsAccountId = 1 };
        var loan = new Loan { Id = 1, Status = LoanStatus.Active };
        var account = new SavingsAccount { Id = 1, Status = AccountStatus.Active, Balance = 50 };
        
        _loanRepoMock.Setup(r => r.GetAllListWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Loan, object>>[]>()))
            .ReturnsAsync(new List<Loan> { loan });
        _savingsAccountRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("La cuenta de ahorros no tiene balance suficiente.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenPaymentExceedsOutstandingDebt()
    {
        // Arrange
        var command = new PagarPrestamoCommand { LoanId = 1, Amount = 300, SavingsAccountId = 1 };
        var loan = new Loan { Id = 1, Status = LoanStatus.Active };
        var installment = new AmortizationInstallment { InstallmentNumber = 1, InstallmentAmount = 100, PaidAmount = 0, Status = InstallmentStatus.Pending };
        loan.Installments.Add(installment);

        var account = new SavingsAccount { Id = 1, Status = AccountStatus.Active, Balance = 500 };
        
        _loanRepoMock.Setup(r => r.GetAllListWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Loan, object>>[]>()))
            .ReturnsAsync(new List<Loan> { loan });
        _savingsAccountRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("El monto a pagar excede la deuda total pendiente del préstamo.", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldAllocatePaymentCorrectly_ToOldestInstallmentsFirst()
    {
        // Arrange
        var command = new PagarPrestamoCommand { LoanId = 1, Amount = 150, SavingsAccountId = 1 };
        var loan = new Loan { Id = 1, Status = LoanStatus.Active, CurrentDebt = 300m };
        
        var inst1 = new AmortizationInstallment { InstallmentNumber = 1, InstallmentAmount = 100m, Interest = 10m, PrincipalAmortization = 90m, PaidAmount = 0, Status = InstallmentStatus.Pending };
        var inst2 = new AmortizationInstallment { InstallmentNumber = 2, InstallmentAmount = 100m, Interest = 10m, PrincipalAmortization = 90m, PaidAmount = 0, Status = InstallmentStatus.Pending };
        var inst3 = new AmortizationInstallment { InstallmentNumber = 3, InstallmentAmount = 100m, Interest = 10m, PrincipalAmortization = 90m, PaidAmount = 0, Status = InstallmentStatus.Pending };
        
        loan.Installments.Add(inst2); // Desordenado para validar ordenamiento por antigüedad
        loan.Installments.Add(inst1);
        loan.Installments.Add(inst3);

        var account = new SavingsAccount { Id = 1, Status = AccountStatus.Active, Balance = 500m };

        _loanRepoMock.Setup(r => r.GetAllListWithIncludeAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Loan, object>>[]>()))
            .ReturnsAsync(new List<Loan> { loan });
        _savingsAccountRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        
        // Cuota 1: Totalmente pagada (100) -> 10 interés, 90 capital amortizado
        Assert.Equal(InstallmentStatus.Paid, inst1.Status);
        Assert.Equal(100m, inst1.PaidAmount);

        // Cuota 2: Parcialmente pagada (50 restantes) -> 10 interés, 40 capital amortizado
        Assert.Equal(InstallmentStatus.PartiallyPaid, inst2.Status);
        Assert.Equal(50m, inst2.PaidAmount);

        // Cuota 3: Pendiente e intacta (0)
        Assert.Equal(InstallmentStatus.Pending, inst3.Status);
        Assert.Equal(0m, inst3.PaidAmount);

        // Deuda total reducida: 300 - 90 (cuota 1) - 40 (cuota 2) = 170
        Assert.Equal(170m, loan.CurrentDebt);

        // Cuenta de ahorro debitada por el monto total (150)
        Assert.Equal(350m, account.Balance);

        // Validar transacciones y unit of work
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _savingsAccountRepoMock.Verify(r => r.UpdateAsync(account), Times.Once);
        _loanRepoMock.Verify(r => r.UpdateAsync(loan), Times.Once);
        _transactionRepoMock.Verify(r => r.AddAsync(It.Is<Transaction>(t => t.Amount == 150m && t.Type == TransactionType.Debit)), Times.Once);
    }
}
