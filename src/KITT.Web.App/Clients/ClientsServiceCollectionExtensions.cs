using KITT.Web.App.Clients.Http;

namespace KITT.Web.App.Clients;

public static class ClientsServiceCollectionExtensions
{
    public static IServiceCollection AddCmsClients(this IServiceCollection services, string clientBaseAddress)
    {
        //services
        //    .AddHttpClient<ISettingsClient, SettingsHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
        //    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        services
            .AddHttpClient<IStreamingsClient, StreamingsHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
            /*.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()*/;

        //services
        //    .AddHttpClient<IProposalsClient, ProposalsHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
        //    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        //services
        //    .AddHttpClient<IMessagesClient, MessagesHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
        //    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        //services
        //    .AddHttpClient<IDashboardClient, DashboardHttpClient>(client => client.BaseAddress = new Uri(clientBaseAddress))
        //    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        return services;
    }
}
