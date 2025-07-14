using System.Security.Claims;

namespace RumarApi.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        string? Name { get; }

        Guid GetUserId();

        string? GetUserEmail();
        string? GetUserFirstName();
        string? GetFullName();

        string? GetTenant();

        bool IsAuthenticated();

        bool IsInRole(string role);

        IEnumerable<Claim>? GetUserClaims();
    }
}
