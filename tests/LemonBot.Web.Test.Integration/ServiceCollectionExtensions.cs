using Microsoft.AspNetCore.Authentication;

namespace LemonBot.Web.Test.Integration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

        return services;
    }
}
