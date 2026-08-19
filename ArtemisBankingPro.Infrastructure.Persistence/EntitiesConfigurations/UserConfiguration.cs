using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Entities.Trades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.Trade)
            .WithOne(t => t.AssociatedUser)
            .HasForeignKey<Trade>(t => t.AssociatedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(u => u.TradeId);
    }
}
