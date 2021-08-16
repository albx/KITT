using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client;

namespace LemonBot.Clients.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTwitchClient(this IServiceCollection services)
        {
            services.AddSingleton<TwitchClient>();
            services.AddSingleton<TwitchClientProxy>();

            return services;
        }
    }
}
