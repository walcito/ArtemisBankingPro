using ArtemisBankingPro.Core.Domain.Entities.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");
            builder.HasKey(l => l.Id);

            builder.Property(l => l.UserId).IsRequired();
            builder.Property(l => l.DisbursedAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(l => l.AnnualInterestRate).IsRequired().HasPrecision(18, 2);
            builder.Property(l => l.TermInMonths).IsRequired();
            builder.Property(l => l.CurrentDebt).IsRequired().HasPrecision(18, 2);
            builder.Property(l => l.Status).IsRequired().HasConversion<int>();

            builder.HasIndex(l => l.UserId);
            builder.HasIndex(l => l.Status);

            // Relaciones
            builder.HasMany(l => l.Installments)
                .WithOne(i => i.Loan)
                .HasForeignKey(i => i.LoanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
