using ArtemisBankingPro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.Configurations
{
    public class TradeConfiguration : IEntityTypeConfiguration<Trade>
    {
        public void Configure(EntityTypeBuilder<Trade> builder)
        {
            builder.HasOne(t => t.AssociatedUser)
                .WithOne(u => u.Trade)
                .HasForeignKey<Trade>(t => t.AssociatedUserId);
        }
    }
}
