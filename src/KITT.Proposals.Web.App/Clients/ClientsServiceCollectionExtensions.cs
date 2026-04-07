using KITT.Proposals.Web.App.Clients.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KITT.Proposals.Web.App.Clients;

public static class ClientsServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddProposalsClients(string apiBaseUrl)
        {
            services
                .AddHttpClient<IProposalsClient, ProposalsHttpClient>(client => client.BaseAddress = new(apiBaseUrl));

            return services;
        }
    }
}
