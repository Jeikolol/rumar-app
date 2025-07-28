using Microsoft.AspNetCore.Identity;

namespace Shared.Entities.Identity
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public string? CreatedBy { get; init; }
        public DateTime CreatedOn { get; init; }
    }
}
