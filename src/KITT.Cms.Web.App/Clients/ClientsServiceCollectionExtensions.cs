using KITT.Cms.Web.App.Clients.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KITT.Cms.Web.App.Clients;

public static class ClientsServiceCollectionExtensions
{
    public static IServiceCollection AddCmsClients(
        this IServiceCollection services,
        string apiBaseUrl)
    {
        services
            .AddHttpClient<IStreamingsClient, StreamingsHttpClient>(client => client.BaseAddress = new(apiBaseUrl));

        return services;
    }
}
