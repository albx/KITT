using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace KITT.Web.Testing.Authentication;

public static class TestJwtTokenProvider
{
    public static string Issuer { get; } = "test.identity";

    public static SecurityKey SecurityKey { get; } = new SymmetricSecurityKey(new byte[256]);

    public static SigningCredentials SigningCredentials { get; } = new(SecurityKey, SecurityAlgorithms.HmacSha256);

    public static JwtSecurityTokenHandler JwtSecurityTokenHandler { get; } = new();
}
