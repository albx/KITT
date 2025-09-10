using KITT.Web.App.Client.Clients.Http;

namespace KITT.Web.App.Client.Clients;

public static class ClientsServiceCollectionExtensions
{
    public static IServiceCollection AddGeneralClients(
        this IServiceCollection services,
        string apiBaseUrl)
    {
        services
            .AddHttpClient<ISettingsClient, SettingsHttpClient>(client => client.BaseAddress = new(apiBaseUrl));

        return services;
    }
}
