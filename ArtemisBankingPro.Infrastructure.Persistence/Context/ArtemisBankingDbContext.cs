using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Context
{
    public class ArtemisBankingDbContext : DbContext
    {
        public ArtemisBankingDbContext(DbContextOptions<ArtemisBankingDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<SavingsAccount> SavingsAccounts { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<AmortizationInstallment> AmortizationInstallments { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CardConsumption> CardConsumptions { get; set; }
        public DbSet<CardProcessingLog> CardProcessingLogs { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<ActivationToken> ActivationTokens { get; set; }
        public DbSet<ResetToken> ResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Trade)
                .WithOne(t => t.AssociatedUser)
                .HasForeignKey<User>(u => u.TradeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtemisBankingDbContext).Assembly);
        }
    }
}