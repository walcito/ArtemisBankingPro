using ArtemisBankingPro.Core.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations.Card
{
    public class CardConsumptionConfiguration : IEntityTypeConfiguration<CardConsumption>
    {
        public void Configure(EntityTypeBuilder<CardConsumption> builder)
        {
            #region Basic Configuration
            builder.ToTable("CardConsumptions");
            builder.HasKey(c => c.Id);
            #endregion

            #region Property Configuration
            builder.Property(c => c.CardId).IsRequired();
            builder.Property(c => c.CommerceId).IsRequired();
            builder.Property(c => c.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(c => c.Status).IsRequired().HasConversion<int>();
            builder.Property(c => c.TransactionDate).IsRequired();
            builder.Property(c => c.Description).HasMaxLength(500);
            builder.Property(c => c.RejectionReason).HasMaxLength(500);
            builder.Property(c => c.CreatedAt).IsRequired();
            #endregion

            #region Index Configuration
            builder.HasIndex(c => c.CardId);
            builder.HasIndex(c => c.CommerceId);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.TransactionDate);
            #endregion

            #region Relationship Configuration
            builder.HasOne(c => c.CreditCard)
                .WithMany(cc => cc.Consumptions)
                .HasForeignKey(c => c.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ProcessingLog)
                .WithMany()
                .HasForeignKey(c => c.ProcessingLogId)
                .OnDelete(DeleteBehavior.SetNull);
            #endregion
        }
    }
}
