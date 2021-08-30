using KITT.Auth.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace KITT.Auth
{
    public static class HostExtensions
    {
        public static async Task<IHost> InitializeAuthAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var dataInitializer = scope.ServiceProvider.GetRequiredService<DataInitializer>();
            await dataInitializer.InitializeAsync();

            return host;
        }
    }
}
