using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Interfaces;

namespace ArtemisBankingPro.Core.Application.Services;

// Generates a unique 9-digit loan number
public class LoanNumberGeneratorService : ILoanNumberGeneratorService
{
    private readonly IGenericRepository<Loan> _loanRepository;

    public LoanNumberGeneratorService(IGenericRepository<Loan> loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<string> GenerateUniqueLoanNumberAsync()
    {
        var loans = await _loanRepository.GetAllAsync();
        string loanNumber;

        do
        {
            var random = new Random();
            loanNumber = random.Next(100_000_000, 999_999_999).ToString();
        }
        while (loans.Any(l => l.LoanNumber == loanNumber));

        return loanNumber;
    }
}
