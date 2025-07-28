using Microsoft.AspNetCore.Http;
using RumarApi.Shared.Auth;

namespace Shared.Infrastructure.RepositoryQueryFilters
{
    public class HeaderService : IHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetGuid(string key)
        {
            string? value = GetString(key);

            Guid.TryParse(value, out var guidValue);

            if (guidValue == default)
            {
                return null;
            }

            return guidValue;
        }

        public string? GetString(string key)
        {
            return _httpContextAccessor.HttpContext?.Request.Headers[key];
        }

        public Guid GetUserId()
        {
            string? stringId = _httpContextAccessor.HttpContext?.User?.GetUserId();

            return stringId is not null ? new Guid(stringId) : Guid.Empty;
        }
    }

    public interface IHeaderService
    {
        string? GetString(string key);
        Guid? GetGuid(string key);
        Guid GetUserId();
    }

}
