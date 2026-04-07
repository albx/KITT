using KITT.Cms.Web.App.Clients.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KITT.Cms.Web.App.Clients;

public static class ClientsServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCmsClients(string apiBaseUrl)
        {
            services.AddHttpClient<IStreamingsClient, StreamingsHttpClient>(client => client.BaseAddress = new(apiBaseUrl));
            services.AddHttpClient<IConnectedChannelsClient, ConnectedChannelsHttpClient>(client => client.BaseAddress = new(apiBaseUrl));

            return services;
        }
    }
}
