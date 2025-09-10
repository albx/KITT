using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using KITT.Web.Testing.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KITT.Web.Testing.Authentication;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestClaimsProvider _claimsProvider;

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TestClaimsProvider claimsProvider) : base(options, logger, encoder)
    {
        _claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity("Test");
        identity.AddClaims(_claimsProvider.Claims);

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
