using KITT.Web.Testing.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace KITT.Web.Testing.DependencyInjection;

public static class TestAuthenticationServiceCollectionExtensions
{
    public static TestAuthenticationBuilder AddTestAuthentication(
        this IServiceCollection services,
        string authenticationScheme = "Test")
    {
        ArgumentNullException.ThrowIfNull(services);
        return new TestAuthenticationBuilder(services, authenticationScheme);
    }

    public static IServiceCollection ConfigureTestJwtOptions(
        this IServiceCollection services,
        string tenantId,
        string appId)
    {
        services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.Configuration = new OpenIdConnectConfiguration
                {
                    Issuer = $"https://sts.windows.net/{tenantId}/",
                };

                options.Authority = $"https://sts.windows.net/{tenantId}/";
                options.Audience = $"api://{appId}";

                // ValidIssuer and ValidAudience is not required, but it helps to define them as otherwise they can be overriden by for example the `user-jwts` tool which will cause the validation to fail
                options.TokenValidationParameters.ValidIssuer = $"https://sts.windows.net/{tenantId}/";
                options.TokenValidationParameters.ValidAudience = $"api://{appId}";
                
                options.Configuration.SigningKeys.Add(TestJwtTokenProvider.SecurityKey);
            });

        return services;
    }
}
