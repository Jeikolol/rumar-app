using Shared.Application.Identity.Tokens;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RumarApp.ApiClient
{
    public class LoginApiService : IApiService<TokenRequest, TokenResponse>
    {
        private readonly HttpClient _httpClient;

        public LoginApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenResponse> ExecuteAsync(TokenRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", request);

            response.EnsureSuccessStatusCode();

            if (response.Content == null)
            {
                throw new InvalidOperationException("Response content is null.");
            }

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize response content.");
            }

            return result;
        }
    }
}
