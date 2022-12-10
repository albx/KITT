using KITT.Web.App.Tools.Clients.Http;
using KITT.Web.App.Tools.Clients.SignalR;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace KITT.Web.App.Tools.Clients;

public static class ClientsServiceCollectionExtensions
{
    public static IServiceCollection AddToolsClients(this IServiceCollection services, string clientBaseAddress)
    {
        services
            .AddHttpClient<IBotClient, BotHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        services
            .AddHttpClient<IStreamingsClient, StreamingsHttpClient>(
                "Tools.StreamingsClient",
                client => client.BaseAddress = new Uri(clientBaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        services.AddScoped<IRealtimeClient, SignalRRealtimeClient>();

        return services;
    }
}
