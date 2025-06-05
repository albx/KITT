using System;
using System.Security.Claims;

namespace KITT.Web.Testing.Security;

public class TestClaimsProvider
{
    public IList<Claim> Claims { get; } = [];

    public TestClaimsProvider()
    {
        Claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
        Claims.Add(new Claim(ClaimTypes.Name, "Test User"));
        Claims.Add(new Claim("oid", Guid.NewGuid().ToString()));
    }
}
