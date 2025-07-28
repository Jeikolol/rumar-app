using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Application.Identity.Tokens;
using Shared.Entities.Identity;
using Shared.Infrastructure.Auth.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RumarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        

        [HttpPost("login")]
        public async Task<TokenResponse> Login(TokenRequest dto, CancellationToken cancellationToken)
        {
            return await _tokenService.GetTokenAsync(dto, GetIpAddress(), cancellationToken);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var header = Request.Headers["X-Forwarded-For"].ToString();

                // Sometimes the header contains multiple IPs separated by commas, take the first one
                var ip = header.Split(',').FirstOrDefault()?.Trim();

                if (!string.IsNullOrEmpty(ip))
                    return ip;
            }

            // Fall back to direct connection IP
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
        }

    }
}
