using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using ApplicationData = Windows.Storage.ApplicationData;

namespace RumarApp.Helpers
{
    public static class TokenStorage
    {
        public static void SaveToken(string token)
        {
            ApplicationData.Current.LocalSettings.Values["JwtToken"] = token;
        }

        public static string? RetrieveToken()
        {
            return ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;
        }

        public static IReadOnlyDictionary<string, string> GetClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
        }
    }
}
