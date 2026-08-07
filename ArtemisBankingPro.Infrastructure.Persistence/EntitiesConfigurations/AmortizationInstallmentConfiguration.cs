using ArtemisBankingPro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations
{
    public class AmortizationInstallmentConfiguration : IEntityTypeConfiguration<AmortizationInstallment>
    {
        public void Configure(EntityTypeBuilder<AmortizationInstallment> builder)
        {
            builder.ToTable("AmortizationInstallments");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.LoanId).IsRequired();
            builder.Property(a => a.InstallmentNumber).IsRequired();
            builder.Property(a => a.InstallmentAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(a => a.Interest).IsRequired().HasPrecision(18, 2);
            builder.Property(a => a.PrincipalAmortization).IsRequired().HasPrecision(18, 2);
            builder.Property(a => a.PendingBalance).IsRequired().HasPrecision(18, 2);
            builder.Property(a => a.Status).IsRequired().HasConversion<int>();
            builder.Property(a => a.PaidAmount).IsRequired().HasPrecision(18, 2);

            builder.HasIndex(a => a.LoanId);
            builder.HasIndex(a => a.Status);
        }
    }
}
