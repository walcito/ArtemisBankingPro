using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Context
{
    public class ArtemisBankingDbContext : DbContext
    {
        public ArtemisBankingDbContext(DbContextOptions<ArtemisBankingDbContext> options) : base(options) { }

 
    public DbSet<ActivationToken> ActivationTokens => Set<ActivationToken>();
    public DbSet<ResetToken> ResetTokens => Set<ResetToken>();
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<CardConsumption> CardConsumptions { get; set; }
    public DbSet<CardProcessingLog> CardProcessingLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtemisBankingDbContext).Assembly);
        }
    }
}