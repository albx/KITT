using System.Net.Http.Json;

namespace LemonBot.Clients;

public class BotClient
{
    public HttpClient Client { get; }

    public BotClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task SendImageOverlayAsync(string resourceUrl)
    {
        await Client.PostAsJsonAsync(
            "api/SendImageOverlay",
            new { resourceUrl });
    }

    public async Task SendTextOverlayAsync(string userName, string message)
    {
        await Client.PostAsJsonAsync(
            "api/SendTextOverlay",
            new { userName, message });
    }
}
