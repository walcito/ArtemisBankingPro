using Microsoft.EntityFrameworkCore;
using ArtemisBankingPro.Core.Domain.Entities;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtemisBankingDbContext).Assembly);
    }
}
