
namespace ArtemisBankingPro.Core.Application.Dtos.Card
{
    public class CreateCreditCardDto
    {
        public required string ClientId { get; set; }
        public required decimal CreditLimit { get; set; }
        public string? Description { get; set; }
    }
}
