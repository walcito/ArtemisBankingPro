using ArtemisBankingPro.Core.Domain.Entities.Trades;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserEntity = ArtemisBankingPro.Core.Domain.Entities.Users.User;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations.User;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        #region Basic Configuration
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        #endregion

        #region Property Configuration
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.IdentificationNumber).IsRequired().HasMaxLength(20);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Role).IsRequired().HasConversion<int>();
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.TradeId);
        #endregion

        #region Index Configuration
        builder.HasIndex(u => u.IdentificationNumber).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();
        #endregion

        #region Relationship Configuration
        builder.HasOne(u => u.Trade)
            .WithOne(t => t.AssociatedUser)
            .HasForeignKey<UserEntity>(u => u.TradeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.SavingsAccounts)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.CreditCards)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<ActivationToken>()
            .WithOne()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<ResetToken>()
            .WithOne()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}
