using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations;

public class SavingsAccountConfiguration : IEntityTypeConfiguration<SavingsAccount>
{
    public void Configure(EntityTypeBuilder<SavingsAccount> builder)
    {
        #region Basic Configuration
        builder.ToTable("SavingsAccounts");
        builder.HasKey(s => s.Id);
        #endregion

        #region Property Configuration
        builder.Property(s => s.AccountNumber)
            .IsRequired()
            .HasMaxLength(9)
            .IsFixedLength();

        builder.Property(s => s.Balance)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0.00m);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.UserId)
            .IsRequired();
        #endregion

        #region Index Configuration
        builder.HasIndex(s => s.AccountNumber)
            .IsUnique();
        #endregion

        #region Relationship Configuration
        builder.HasOne(s => s.User)
            .WithMany(u => u.SavingsAccounts)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}