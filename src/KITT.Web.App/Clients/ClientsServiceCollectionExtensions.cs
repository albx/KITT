using KITT.Web.App.Clients.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace KITT.Web.App.Clients;

public static class ClientsServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsClient(this IServiceCollection services, string clientBaseAddress)
    {
        services
            .AddHttpClient<ISettingsClient, SettingsHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        return services;
    }

    public static IServiceCollection AddStreamingsClient(this IServiceCollection services, string clientBaseAddress)
    {
        services
            .AddHttpClient<IStreamingsClient, StreamingsHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        return services;
    }

    public static IServiceCollection AddToolsClients(this IServiceCollection services, string clientBaseAddress)
    {
        services
            .AddHttpClient<IBotClient, BotHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        return services;
    }
}
