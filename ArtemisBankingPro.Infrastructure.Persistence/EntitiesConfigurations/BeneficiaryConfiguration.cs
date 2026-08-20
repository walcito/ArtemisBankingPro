using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations;

public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        #region Basic Configuration
        builder.ToTable("Beneficiaries");
        builder.HasKey(b => b.Id);
        #endregion

        #region Property Configuration
        builder.Property(b => b.OwnerUserId).IsRequired();
        builder.Property(b => b.BeneficiaryAccountNumber).IsRequired().HasMaxLength(50);
        builder.Property(b => b.Alias).HasMaxLength(100);
        #endregion

        #region Index Configuration
        builder.HasIndex(b => new { b.OwnerUserId, b.BeneficiaryAccountNumber }).IsUnique();
        #endregion

        #region Relationship Configuration
        builder.HasOne(b => b.OwnerUser)
            .WithMany()
            .HasForeignKey(b => b.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}
