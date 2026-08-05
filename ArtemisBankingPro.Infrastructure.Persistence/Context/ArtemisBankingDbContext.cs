using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Context;

public class ArtemisBankingDbContext : DbContext
{
    public ArtemisBankingDbContext(DbContextOptions<ArtemisBankingDbContext> options) : base(options) { }

    public DbSet<CuentaAhorro> CuentasAhorro => Set<CuentaAhorro>();
    public DbSet<Comercio> Comercios => Set<Comercio>();
    public DbSet<Beneficiario> Beneficiarios => Set<Beneficiario>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
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
