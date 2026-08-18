using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Context
{
    public class ArtemisBankingDbContext : DbContext
    {
        public ArtemisBankingDbContext(DbContextOptions<ArtemisBankingDbContext> options) : base(options) { }

 
    public DbSet<ActivationToken> ActivationTokens => Set<ActivationToken>();
    public DbSet<ResetToken> ResetTokens => Set<ResetToken>();
    public DbSet<Loan> Loans { get; set; }
    public DbSet<AmortizationInstallment> AmortizationInstallments { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<CardConsumption> CardConsumptions { get; set; }
    public DbSet<CardProcessingLog> CardProcessingLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //pendiente a ver si funciona la migracion ya restableci la relacion con trade y user by waldin
            modelBuilder.Entity<User>()
                .HasOne(u => u.Trade)
                .WithOne(t => t.AssociatedUser)
                .HasForeignKey<User>(u => u.TradeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtemisBankingDbContext).Assembly);
        }
    }
}