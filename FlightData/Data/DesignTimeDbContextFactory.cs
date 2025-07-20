// FlightData/Data/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlightData.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // Usamos SQLite en un archivo local
            optionsBuilder.UseSqlite("Data Source=FlightDb.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
