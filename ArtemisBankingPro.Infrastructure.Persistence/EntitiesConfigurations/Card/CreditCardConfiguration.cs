using ArtemisBankingPro.Core.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations.Card
{
    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            #region Basic Configuration
            builder.ToTable("CreditCards");
            builder.HasKey(c => c.Id);
            #endregion

            #region Property Configuration
            builder.Property(c => c.UserId).IsRequired();
            builder.Property(c => c.CardNumber).IsRequired().HasMaxLength(16);
            builder.Property(c => c.ExpirationDate).IsRequired();
            builder.Property(c => c.CvcHash).IsRequired().HasMaxLength(255);
            builder.Property(c => c.CreditLimit).IsRequired().HasPrecision(18, 2);
            builder.Property(c => c.CurrentDebt).IsRequired().HasPrecision(18, 2);
            builder.Property(c => c.Status).IsRequired().HasConversion<int>();
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.CancelledAt);
            builder.Property(c => c.ModifiedAt).IsRequired();
            #endregion

            #region Index Configuration
            builder.HasIndex(c => c.CardNumber).IsUnique();
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => c.Status);
            #endregion

            #region Relationship Configuration
            builder.HasMany(c => c.Consumptions)
                .WithOne(cc => cc.CreditCard)
                .HasForeignKey(cc => cc.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ProcessingLogs)
                .WithOne(pl => pl.CreditCard)
                .HasForeignKey(pl => pl.CardId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
