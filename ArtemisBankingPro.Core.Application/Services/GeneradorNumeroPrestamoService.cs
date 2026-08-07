using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;

namespace ArtemisBankingPro.Core.Application.Services;

// Genera un número de préstamo único de 9 dígitos
public class GeneradorNumeroPrestamoService
{
    private readonly IGenericRepository<Loan> _loanRepository;

    public GeneradorNumeroPrestamoService(IGenericRepository<Loan> loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<string> GenerarNumeroUnicoAsync()
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
