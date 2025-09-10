using Microsoft.Identity.Web;
using System.Security.Claims;

namespace KITT.Web.Shared.Security;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
        => user.GetObjectId()!;
}
