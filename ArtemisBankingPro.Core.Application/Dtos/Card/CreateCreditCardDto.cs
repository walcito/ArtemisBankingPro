
namespace ArtemisBankingPro.Core.Application.Dtos.Card
{
    public class CreateCreditCardDto
    {
        public required int ClientId { get; set; }
        public required decimal CreditLimit { get; set; }
        public string? Description { get; set; }
    }
}
