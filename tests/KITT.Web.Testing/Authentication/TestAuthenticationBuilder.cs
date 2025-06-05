using System;
using KITT.Web.Testing.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KITT.Web.Testing.Authentication;

public sealed class TestAuthenticationBuilder
{
    public IServiceCollection Services { get; }
    public string AuthenticationScheme { get; }

    internal TestAuthenticationBuilder(IServiceCollection services, string authenticationScheme)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        AuthenticationScheme = authenticationScheme;

        ConfigureClaimsProvider();
        ConfigureAuthentication();
    }

    private void ConfigureAuthentication()
    {
        Services.AddAuthentication(AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(AuthenticationScheme, null);
    }

    private void ConfigureClaimsProvider()
    {
        Services.TryAddScoped<TestClaimsProvider>();
    }
}
