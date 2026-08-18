using ArtemisBankingPro.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations.User;

public class ResetTokenConfiguration : IEntityTypeConfiguration<ResetToken>
{
    public void Configure(EntityTypeBuilder<ResetToken> builder)
    {
        #region Basic Configuration
        builder.ToTable("ResetTokens");
        builder.HasKey(t => t.Id);
        #endregion

        #region Property Configuration
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.Token).IsRequired().HasMaxLength(500);
        builder.Property(t => t.ExpirationDate).IsRequired();
        builder.Property(t => t.IsUsed).IsRequired();
        #endregion

        #region Index Configuration
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.Token);
        #endregion
    }
}
