using System.Security.Claims;

namespace RumarApi.Shared.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetEmail(this ClaimsPrincipal principal)
            => principal.FindFirstValue(RumarClaims.Email);

        public static string? GetFullName(this ClaimsPrincipal principal)
            => principal?.FindFirst(RumarClaims.Fullname)?.Value;

        public static string? GetFirstName(this ClaimsPrincipal principal)
            => principal?.FindFirst(RumarClaims.Firstname)?.Value;

        public static string? GetSurname(this ClaimsPrincipal principal)
            => principal?.FindFirst(RumarClaims.Lastname)?.Value;

        public static string? GetPhoneNumber(this ClaimsPrincipal principal)
            => principal.FindFirstValue(RumarClaims.PhoneNumber);

        public static string? GetUserId(this ClaimsPrincipal principal)
           => principal.FindFirstValue(RumarClaims.UserIdentifier);

        public static string? GetImageUrl(this ClaimsPrincipal principal)
           => principal.FindFirstValue(RumarClaims.ImageUrl);

        public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal) =>
            DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(
                principal.FindFirstValue(RumarClaims.Expiration)));

        private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
            principal is null
                ? throw new ArgumentNullException(nameof(principal))
                : principal.FindFirst(claimType)?.Value;
    }
}
