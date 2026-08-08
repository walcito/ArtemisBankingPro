namespace ArtemisBankingPro.Core.Application.Dtos.User;

public class EditUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public decimal? AdditionalAmount { get; set; }
}
