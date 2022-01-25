using KITT.Web.Models.Tools;
using LemonBot.Web.Configuration;
using Microsoft.Extensions.Options;

namespace LemonBot.Web.Areas.Tools.Services;

public class BotHttpClient
{
    public BotHttpClient(HttpClient client)
    {
        Client = client;
    }

    public HttpClient Client { get; }

    ///api/continuouswebjobs/{job name}
    public async Task<BotJobDetail> GetDetailAsync()
    {
        var detail = await Client.GetFromJsonAsync<BotJobDetail>("/api/continuouswebjobs/LemonBot");
        if (detail is null)
        {
            throw new InvalidOperationException("Cannot find job details");
        }

        return detail;
    }

    public async Task StartAsync()
    {
        var response = await Client.PostAsync("/api/continuouswebjobs/LemonBot/start", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error starting bot");
        }
    }

    public async Task StopAsync()
    {
        var response = await Client.PostAsync("/api/continuouswebjobs/LemonBot/stop", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error starting bot");
        }
    }
}
