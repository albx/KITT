using KITT.Web.Models.Proposals;
using System.Net.Http.Json;

namespace KITT.Web.App.Clients.Http;

public class ProposalsHttpClient : IProposalsClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "/api/console/proposals";

    public ProposalsHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<ProposalListModel> GetAllProposalsAsync(ProposalsQueryModel query)
    {
        var url = ApiResource;
        var queryString = query.ToQueryString();
        if (!string.IsNullOrWhiteSpace(queryString))
        {
            url = $"{url}?{queryString}";
        }

        var model = await Client.GetFromJsonAsync<ProposalListModel>(url);
        return model ?? new();
    }
}
