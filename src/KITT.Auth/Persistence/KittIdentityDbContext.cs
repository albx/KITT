using IdentityServer4.EntityFramework.Options;
using KITT.Auth.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KITT.Auth.Persistence
{
    public class KittIdentityDbContext : ApiAuthorizationDbContext<KittUser>
    {
        public KittIdentityDbContext(
            DbContextOptions options, 
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
    }
}
