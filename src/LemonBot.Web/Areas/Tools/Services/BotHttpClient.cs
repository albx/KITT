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
