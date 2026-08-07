using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ArtemisBankingPro.Infrastructure.Persistence.Context
{
    public class ArtemisBankingDbContextFactory : IDesignTimeDbContextFactory<ArtemisBankingDbContext>
    {
        public ArtemisBankingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ArtemisBankingDbContext>();
            
            // Cadena de conexión por defecto solo para crear las migraciones (tiempo de diseño)
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ArtemisBankingDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ArtemisBankingDbContext(optionsBuilder.Options);
        }
    }
}
