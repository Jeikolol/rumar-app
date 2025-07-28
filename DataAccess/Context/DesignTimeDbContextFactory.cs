using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.Application.Common.Interfaces;
using Shared.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ICurrentUser CurrentUser;
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath($"{Directory.GetCurrentDirectory()}/Configurations")
                .AddJsonFile("database.json", optional: false)
                .Build();

            var dbSettings = config
                .GetSection(nameof(DatabaseSettings))
                .Get<DatabaseSettings>();

            if (string.IsNullOrWhiteSpace(dbSettings?.ConnectionString) || string.IsNullOrWhiteSpace(dbSettings.DBProvider))
                throw new InvalidOperationException("ConnectionString or DBProvider is missing in appsettings.Development.json");

            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Use your custom extension for UseDatabase
            dbOptions.UseDatabase(dbSettings.DBProvider, dbSettings.ConnectionString);

            // Wrap it in Options to match constructor signature
            return new ApplicationDbContext(dbOptions.Options, Options.Create(dbSettings), CurrentUser);
        }
    }
}
