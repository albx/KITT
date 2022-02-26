using KITT.Core.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace LemonBot.Web.Test.Integration.Fixtures;

public class KittWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //builder.ConfigureTestServices(services =>
        //{
        //    services.AddAuthentication("Test")
        //        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
        //});

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<KittDbContext>(options => options.UseInMemoryDatabase("Kitt-InMemory-Test"));

            var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<KittDbContext>();

            if (context.Streamings.Any())
            {
                context.Streamings.RemoveRange(context.Streamings);
                context.SaveChanges();
            }
        });
    }
}
