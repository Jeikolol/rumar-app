using Microsoft.EntityFrameworkCore;
using RumarApi.Shared.Auth;
using Shared.Application.Common.Exceptions;
using Shared.Application.Common.Models;

namespace Api.Identity.Users;

internal partial class UserService
{
    public async Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new UnauthorizedException("Authentication Failed.");

        var userRoles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();

        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == RumarClaims.Permission)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken));
        }

        return permissions.Distinct().ToList();
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken)
    {
        var permissions = await GetPermissionsAsync(userId, cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    public async Task<bool> HasPermissionWithCredentialsAsync(UserCredentials credentials, string permission, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentNullException("Permiso Invalido");
        }

        var userResult = await _userManager.FindByEmailAsync(credentials.UserName.Trim().Normalize());

        if (userResult == null
            || !await _userManager.CheckPasswordAsync(userResult, credentials.Password))
        {

            throw new NotFoundException("Usuario/Contraseña incorrectas.");
        }

        return await HasPermissionAsync(userResult.Id, permission, cancellationToken);
    }
}