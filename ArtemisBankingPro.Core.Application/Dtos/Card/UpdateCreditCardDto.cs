
namespace ArtemisBankingPro.Core.Application.Dtos.Card
{
    public class UpdateCreditCardDto
    {
        public required decimal CreditLimit { get; set; }
        public string? Description { get; set; }
    }
}
