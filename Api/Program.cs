using CSSCloudErp.WebApi.Host.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RumarApi.Context;
using RumarApi.Entities.Identity;
using RumarApi.Infrastructure;
using RumarApi.Infrastructure.Auth.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurations();

// Add services to the container.
var services = builder.Services;
var config = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                e => e.MigrationsAssembly("Migrations")
                .MigrationsHistoryTable("__EFMigrationsHistory", "SqlServer")),
        ServiceLifetime.Transient)
    .AddTransient<ApplicationDbInitializer>()
    .AddTransient<ApplicationDbSeeder>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

services.AddServices();
services.AddJwtAuth(config);

builder.Services.AddControllers();
services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
