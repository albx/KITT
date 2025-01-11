using KITT.Proposals.Web.Models;
using System.Net.Http.Json;

namespace KITT.Proposals.Web.App.Clients.Http;

public class ProposalsHttpClient : IProposalsClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "/api/proposals";

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

    public async Task AcceptProposalAsync(Guid proposalId)
    {
        var url = $"{ApiResource}/{proposalId}";
        var response = await Client.PatchAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error accepting proposal");
        }
    }

    public async Task RejectProposalAsync(Guid proposalId)
    {
        var url = $"{ApiResource}/{proposalId}";
        var response = await Client.DeleteAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error accepting proposal");
        }
    }

    public async Task RefuseProposalAsync(Guid proposalId)
    {
        var url = $"{ApiResource}/{proposalId}/refuse";
        var response = await Client.DeleteAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error accepting proposal");
        }
    }

    public async Task<ProposalDetailModel?> GetProposalDetailAsync(Guid proposalId)
    {
        var url = $"{ApiResource}/{proposalId}";
        var model = await Client.GetFromJsonAsync<ProposalDetailModel>(url);

        return model;
    }

    public async Task ScheduleProposalAsync(Guid proposalId, ScheduleProposalModel model)
    {
        var url = $"{ApiResource}/{proposalId}/schedule";

        var response = await Client.PostAsJsonAsync(url, model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error scheduling proposal");
        }
    }

    public async Task<ProposalsStatsModel> GetProposalsStatsAsync()
    {
        var proposalsStats = await Client.GetFromJsonAsync<ProposalsStatsModel>($"{ApiResource}/stats");
        return proposalsStats ?? new(0, 0);
    }
}
