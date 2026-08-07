using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Context;

public class ArtemisBankingDbContext : DbContext
{
    public ArtemisBankingDbContext(DbContextOptions<ArtemisBankingDbContext> options) : base(options) { }

    public DbSet<SavingsAccount> CuentasAhorro => Set<SavingsAccount>();
    public DbSet<Trade> Comercios => Set<Trade>();
    public DbSet<Beneficiary> Beneficiarios => Set<Beneficiary>();
    public DbSet<Transaction> Transacciones => Set<Transaction>();
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

        // Fix for the 1-to-1 relationship ambiguity between Trade and User
        modelBuilder.Entity<Trade>()
            .HasOne(t => t.AssociatedUser)
            .WithOne(u => u.Trade)
            .HasForeignKey<Trade>(t => t.AssociatedUserId);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtemisBankingDbContext).Assembly);
    }
}
