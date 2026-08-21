using ArtemisBankingPro.Core.Domain.Entities.Trades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations
{
    public class TradeConfiguration : IEntityTypeConfiguration<Trade>
    {
        public void Configure(EntityTypeBuilder<Trade> builder)
        {
            builder.ToTable("Trades");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).IsRequired().HasMaxLength(150);
            builder.Property(t => t.Description).HasMaxLength(500);
            builder.Property(t => t.Mail).IsRequired().HasMaxLength(150);
            builder.Property(t => t.Phone).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Rnc).IsRequired().HasMaxLength(50);
            builder.Property(t => t.IsActive).IsRequired();

            builder.HasIndex(t => t.Rnc).IsUnique();
            builder.HasIndex(t => t.Mail);

            builder.HasOne(t => t.AssociatedUser)
                .WithOne(u => u.Trade)
                .HasForeignKey<Trade>(t => t.AssociatedUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
