using KITT.Web.Testing.Authentication;
using Microsoft.Extensions.DependencyInjection;

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
}
