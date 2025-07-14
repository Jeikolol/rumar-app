namespace RumarApi.Application.Identity.Tokens
{
    public record RefreshTokenRequest(string Token, string RefreshToken);
}
