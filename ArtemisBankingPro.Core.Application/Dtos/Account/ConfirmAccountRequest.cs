namespace ArtemisBankingPro.Core.Application.Dtos.Account;

public class ConfirmAccountRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}