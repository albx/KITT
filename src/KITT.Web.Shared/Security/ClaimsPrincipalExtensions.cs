using Microsoft.Identity.Web;
using System.Security.Claims;

namespace KITT.Web.Shared.Security;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal user)
    {
        public string GetUserId() => user.GetObjectId()!;
    }
}
