using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Shared.Infrastructure.Identity
{
    public static class IdentityResultExtensions
    {
        public static List<string> GetErrors(this IdentityResult result, IStringLocalizer T) =>
            result.Errors.Select(e => T[e.Description].ToString()).ToList();
    }
}
