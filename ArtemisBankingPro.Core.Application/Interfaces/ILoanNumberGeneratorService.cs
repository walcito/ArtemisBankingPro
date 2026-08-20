namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface ILoanNumberGeneratorService
{
    Task<string> GenerateUniqueLoanNumberAsync();
}
