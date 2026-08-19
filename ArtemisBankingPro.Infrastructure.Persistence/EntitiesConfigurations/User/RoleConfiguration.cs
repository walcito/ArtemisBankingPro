using ArtemisBankingPro.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBankingPro.Infrastructure.Persistence.EntitiesConfigurations.User;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        #region Basic Configuration
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        #endregion

        #region Property Configuration
        builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
        #endregion

        #region Index Configuration
        builder.HasIndex(r => r.Name).IsUnique();
        #endregion
    }
}
