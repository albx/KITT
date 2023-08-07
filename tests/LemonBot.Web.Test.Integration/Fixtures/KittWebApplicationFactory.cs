using KITT.Core.Persistence;
using KITT.Telegram.Messages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LemonBot.Web.Test.Integration.Fixtures;

public class KittWebApplicationFactory : WebApplicationFactory<Program>
{
    private Mock<IMessageBus> _messageBusMock = new Mock<IMessageBus>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveDbContextDescriptor(services);
            services.AddDbContext<KittDbContext>(options => options.UseInMemoryDatabase("Kitt-InMemory-Test"));

            RemoveMessageBusDescriptor(services);
            services.AddSingleton(_messageBusMock.Object);
        });
    }

    private void RemoveMessageBusDescriptor(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageBus));
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }

    private void RemoveDbContextDescriptor(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<KittDbContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }


}
