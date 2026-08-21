namespace ArtemisBankingPro.Core.Application.Dtos.User;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public int DomainUserId { get; set; }           
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? CommerceId { get; set; }
}