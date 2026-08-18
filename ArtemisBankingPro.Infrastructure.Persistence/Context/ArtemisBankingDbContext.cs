using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Context
{
    public class ArtemisBankingDbContext : DbContext
    {
        public ArtemisBankingDbContext(DbContextOptions<ArtemisBankingDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<SavingsAccount> SavingsAccounts => Set<SavingsAccount>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<AmortizationInstallment> AmortizationInstallments => Set<AmortizationInstallment>();
        public DbSet<CreditCard> CreditCards => Set<CreditCard>();
        public DbSet<CardConsumption> CardConsumptions => Set<CardConsumption>();
        public DbSet<CardProcessingLog> CardProcessingLogs => Set<CardProcessingLog>();
        public DbSet<Trade> Trades => Set<Trade>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
        public DbSet<ActivationToken> ActivationTokens => Set<ActivationToken>();
        public DbSet<ResetToken> ResetTokens => Set<ResetToken>();

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