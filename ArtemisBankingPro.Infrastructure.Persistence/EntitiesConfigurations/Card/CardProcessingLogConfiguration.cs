using ArtemisBankingPro.Core.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations.Card
{
    public class CardProcessingLogConfiguration : IEntityTypeConfiguration<CardProcessingLog>
    {
        public void Configure(EntityTypeBuilder<CardProcessingLog> builder)
        {
            #region Basic Configuration
            builder.ToTable("CardProcessingLogs");
            builder.HasKey(c => c.Id);
            #endregion

            #region Property Configuration
            builder.Property(c => c.CardId).IsRequired();
            builder.Property(c => c.CommerceId).IsRequired();
            builder.Property(c => c.RequestedAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(c => c.ProcessingStatus).IsRequired().HasConversion<int>();
            builder.Property(c => c.ValidationErrors).HasMaxLength(1000);
            builder.Property(c => c.ProcessedAt).IsRequired();
            builder.Property(c => c.IpAddress).HasMaxLength(45);
            builder.Property(c => c.UserAgent).HasMaxLength(500);
            builder.Property(c => c.CreatedAt).IsRequired();
            #endregion

            #region Index Configuration
            builder.HasIndex(c => c.CardId);
            builder.HasIndex(c => c.CommerceId);
            builder.HasIndex(c => c.ProcessingStatus);
            builder.HasIndex(c => c.ProcessedAt);
            #endregion

            #region Relationship Configuration
            builder.HasOne(c => c.CreditCard)
                .WithMany(cc => cc.ProcessingLogs)
                .HasForeignKey(c => c.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ApprovedConsumption)
                .WithOne(cc => cc.ProcessingLog)
                .HasForeignKey<CardProcessingLog>(c => c.ApprovedConsumptionId)
                .OnDelete(DeleteBehavior.SetNull);
            #endregion
        }
    }
}
