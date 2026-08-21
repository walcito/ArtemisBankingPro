using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(t => t.Type).IsRequired().HasConversion<int>();
            builder.Property(t => t.Status).IsRequired().HasConversion<int>();
            builder.Property(t => t.Date).IsRequired();
            builder.Property(t => t.SavingsAccountId).IsRequired();
            builder.Property(t => t.PerformedByCashierId);

            builder.HasIndex(t => t.SavingsAccountId);
            builder.HasIndex(t => t.PerformedByCashierId);
            builder.HasIndex(t => t.Date);

            builder.HasOne(t => t.SavingsAccount)
                .WithMany()
                .HasForeignKey(t => t.SavingsAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.PerformedByCashier)
                .WithMany()
                .HasForeignKey(t => t.PerformedByCashierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
