using KITT.Proposals.Web.App.Clients.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KITT.Proposals.Web.App.Clients;

public static class ClientsServiceCollectionExtensions
{
    public static IServiceCollection AddProposalsClients(
        this IServiceCollection services,
        string apiBaseUrl)
    {
        services.AddHttpClient<IProposalsClient, ProposalsHttpClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        return services;
    }
}
