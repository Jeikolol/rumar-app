using Api.Application;
using Api.Auth;
using Api.Caching;
using Api.Data;
using CSSCloudErp.WebApi.Host.Configurations;
using DataAccess.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RumarApi.Infrastructure;
using Shared.Application.Common.Exceptions;
using Shared.Entities.Identity;
using Shared.Infrastructure.Auth;
using Shared.Infrastructure.Auth.Jwt;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurations();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(nameof(DatabaseSettings))
);
// Add services to the container.
var services = builder.Services;
var config = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>((p, m) =>
{
    var dbSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    m.UseSqlServer(
        dbSettings.ConnectionString,
        e => e.MigrationsAssembly("Migrator"));
}, ServiceLifetime.Transient)
.AddTransient<ApplicationDbInitializer>()
.AddTransient<ApplicationDbSeeder>();

services.AddApplication();
services.AddServices();
services.AddAuth(config);
services.AddCaching(config);

builder.Services.AddControllers();
services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var init = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();
        await init.InitializeAsync(CancellationToken.None);
    }
    catch (Exception ex)
    {
        //logger.Error(ex, "Error during DB initialization.");
        throw new CustomException(ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCurrentUser();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
