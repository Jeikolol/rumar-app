using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context
{
    internal static class Startup
    {
        internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
        {
            switch (dbProvider.ToLowerInvariant())
            {
                case "mssql":
                    return builder.UseSqlServer(connectionString, e =>
                         e.MigrationsAssembly("Migrator"));

                default:
                    throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
            }
        }
    }
}
