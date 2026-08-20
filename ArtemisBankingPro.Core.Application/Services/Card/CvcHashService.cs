using ArtemisBankingPro.Core.Application.Interfaces.Card;
using System.Security.Cryptography;
using System.Text;

namespace ArtemisBankingPro.Core.Application.Services.Card
{
    public class CvcHashService : ICvcHashService
    {
        public (string PlainCvc, string HashedCvc) GenerateHashedCvc()
        {
            var plainCvc = RandomNumberGenerator.GetInt32(100, 1000).ToString();
            var hashedCvc = Hash(plainCvc);
            return (plainCvc, hashedCvc);
        }
        public bool VerifyCvc(string plainCvc, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(plainCvc) || string.IsNullOrWhiteSpace(storedHash))
                return false;
            var computedHash = Hash(plainCvc);
            return string.Equals(computedHash, storedHash, StringComparison.OrdinalIgnoreCase);
        }
        public string Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
