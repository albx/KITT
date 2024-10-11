using KITT.Web.Models.Dashboard;
using System.Net.Http.Json;

namespace KITT.Web.App.Clients.Http;

public class DashboardHttpClient : IDashboardClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "api/dashboard";

    public DashboardHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<DashboardModel> GetDashboardStatsAsync()
    {
        var model = await Client.GetFromJsonAsync<DashboardModel>(ApiResource);
        return model ?? new();
    }
}
