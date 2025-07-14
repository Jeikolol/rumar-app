using Microsoft.AspNetCore.Identity;
using RumarApi.Context;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RumarApi.Infrastructure
{
    internal class ApplicationDbSeeder
    {
        public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
        }
    }
}
