using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations
{
    public class SavingsAccountConfiguration : IEntityTypeConfiguration<SavingsAccount>
    {
        public void Configure(EntityTypeBuilder<SavingsAccount> builder)
        {
            builder.ToTable("SavingsAccounts");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.AccountNumber).IsRequired().HasMaxLength(9);
            builder.Property(s => s.Balance).IsRequired().HasPrecision(18, 2);
            builder.Property(s => s.Type).IsRequired().HasConversion<int>();
            builder.Property(s => s.Status).IsRequired().HasConversion<int>();
            builder.Property(s => s.UserId).IsRequired();

            builder.HasIndex(s => s.AccountNumber).IsUnique();
            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => s.Status);

            builder.HasOne(s => s.User)
                .WithMany(u => u.SavingsAccounts)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
