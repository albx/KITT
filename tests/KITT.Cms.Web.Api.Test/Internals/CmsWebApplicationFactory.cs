using KITT.Core.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace KITT.Cms.Web.Api.Test.Internals;

public class CmsWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            ReplaceDbContext(services);
        });
    }

    private static void ReplaceDbContext(IServiceCollection services)
    {
        var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<KittDbContext>));
        if (dbContextDescriptor is not null)
        {
            services.Remove(dbContextDescriptor);
        }

        services.AddDbContextPool<KittDbContext>(
            options => options.UseInMemoryDatabase("Kitt-InMemory"));
    }
}
