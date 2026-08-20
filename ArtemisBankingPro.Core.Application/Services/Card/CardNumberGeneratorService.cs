using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using System.Security.Cryptography;

namespace ArtemisBankingPro.Core.Application.Services.Card
{
    public class CardNumberGeneratorService : ICardNumberGeneratorService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        public CardNumberGeneratorService(ICreditCardRepository creditCardRepository)
        {
            _creditCardRepository = creditCardRepository;
        }
        public async Task<string> GenerateUniqueCardNumberAsync()
        {
            string cardNumber;
            do
            {
                cardNumber = GenerateLuhnCardNumber();
            }
            while (await _creditCardRepository.CardNumberExistsAsync(cardNumber));
            return cardNumber;
        }
        private static string GenerateLuhnCardNumber()
        {
            var first15 = "4" + string.Concat(Enumerable.Range(0, 14).Select(_ => RandomNumberGenerator.GetInt32(0, 10).ToString()));
            var checksum = CalculateLuhnChecksum(first15);
            return first15 + checksum;
        }
        private static int CalculateLuhnChecksum(string numberWithoutChecksum)
        {
            int sum = 0;
            for (int i = 0; i < numberWithoutChecksum.Length; i++)
            {
                int digit = numberWithoutChecksum[i] - '0';
                if (i % 2 == 0)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
            }
            return (10 - (sum % 10)) % 10;
        }
    }
}
    

