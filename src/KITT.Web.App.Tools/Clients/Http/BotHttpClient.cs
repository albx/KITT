using KITT.Web.Models.Tools;
using System.Net.Http.Json;

namespace KITT.Web.App.Tools.Clients.Http;

public class BotHttpClient : IBotClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "/api/tools/bot";

    public BotHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task StartAsync()
    {
        var response = await Client.PostAsync($"{ApiResource}/start", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error scheduling streaming");
        }
    }

    public async Task StopAsync()
    {
        var response = await Client.PostAsync($"{ApiResource}/stop", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error scheduling streaming");
        }
    }

    public async Task<BotJobDetail?> GetDetailsAsync()
    {
        var response = await Client.GetFromJsonAsync<BotJobDetail>(ApiResource);
        return response;
    }
}
