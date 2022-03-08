using KITT.Core.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace LemonBot.Web.Test.Integration.Fixtures;

public class KittWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveDbContextDescriptor(services);
            services.AddDbContext<KittDbContext>(options => options.UseInMemoryDatabase("Kitt-InMemory-Test"));
        });
    }

    private void RemoveDbContextDescriptor(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<KittDbContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }


}
