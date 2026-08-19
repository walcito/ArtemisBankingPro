
namespace ArtemisBankingPro.Core.Application.Interfaces.Card
{
    public interface ICvcHashService
    {
        (string PlainCvc, string HashedCvc) GenerateHashedCvc();
        bool VerifyCvc(string plainCvc, string storedHash);
        string Hash(string input);
    }
}

