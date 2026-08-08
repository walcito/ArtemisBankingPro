using ArtemisBankingPro.Core.Domain.Common;

namespace ArtemisBankingPro.Core.Domain.Entities.Users;

public class ResetToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime ExpirationDate { get; set; }
}