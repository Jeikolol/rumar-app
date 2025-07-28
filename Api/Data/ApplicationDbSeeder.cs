using DataAccess.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RumarApi.Shared.Auth;
using Shared.Entities.Identity;
using Shared.Shared.Constants;

namespace Api.Data
{
    internal class ApplicationDbSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationDbSeeder(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            await SeedRolesAsync(dbContext);
            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync(ApplicationDbContext dbContext)
        {
            foreach (string roleName in RumarRoles.DefaultRoles)
            {
                if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                    is not ApplicationRole role)
                {
                    role = new ApplicationRole(roleName);
                    await _roleManager.CreateAsync(role);
                }

                // Assign permissions
                if (roleName == RumarRoles.Basic)
                {
                    await AssignPermissionsToRoleAsync(dbContext, RumarPermissions.Basic, role);
                }
                else if (roleName == RumarRoles.Admin)
                {
                    await AssignPermissionsToRoleAsync(dbContext, RumarPermissions.Admin, role);
                }
            }
        }

        private async Task AssignPermissionsToRoleAsync(ApplicationDbContext dbContext, IReadOnlyList<RumarPermission> permissions, ApplicationRole role)
        {
            var currentClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var permission in permissions.Select(p => p.Name))
            {
                if (!currentClaims.Any(c => c.Type == RumarClaims.Permission && c.Value == permission))
                {
                    await dbContext.RoleClaims
                        .AddAsync(new ApplicationRoleClaim
                          {
                                RoleId = role.Id,
                                ClaimType = RumarClaims.Permission,
                                ClaimValue = permission,
                                CreatedBy = "ApplicationDbSeeder"
                          });

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            bool exists = await _userManager.Users.AnyAsync();

            if (!exists)
            {
                var adminUser = new ApplicationUser
                {
                    Id = EntityConstants.AdminId.ToString(),
                    FirstName = "Administrador",
                    LastName = "Sistema",
                    Email = "jeikoj01@gmail.com",
                    UserName = "Admin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    NormalizedEmail = "jeikoj01@gmail.com".ToUpperInvariant(),
                    NormalizedUserName = "Admin".ToUpperInvariant(),
                    IsActive = true
                };

                var password = new PasswordHasher<ApplicationUser>();
                adminUser.PasswordHash = password.HashPassword(adminUser, EntityConstants.DefaultPassword);

                await _userManager.CreateAsync(adminUser);

                // Assign role to user
                if (!await _userManager.IsInRoleAsync(adminUser, RumarRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(adminUser, RumarRoles.Admin);
                }
            }

            
        }
    }
}
