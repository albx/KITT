using System.Net.Http.Json;
using TwitchLib.Client.Models;

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

    public async Task NotifyStartAsync()
    {
        await Client.PostAsJsonAsync(
            "api/NotifyStart",
            new { startTime = DateTime.UtcNow });
    }

    public async Task NotifyStopAsync()
    {
        await Client.PostAsJsonAsync(
            "api/NotifyStart",
            new { stopTime = DateTime.UtcNow });
    }

    public async Task SendNewUserSubscriptionAsync(string subscriberName)
    {
        await Client.PostAsJsonAsync(
            "api/SendNewUserSubscription",
            new { subscriberName });
    }

    public async Task SendUserLeftAsync(string userName)
    {
        await Client.PostAsJsonAsync(
            "api/SendUserLeft",
            new { userName });
    }

    public async Task SendUserJoinAsync(string userName)
    {
        await Client.PostAsJsonAsync(
            "api/SendUserJoin",
            new { userName });
    }
}
