using System.Net.Http.Json;

namespace LemonBot.Commands;

[BotCommand("!image", HelpText = "Show an image in overlay")]
public class ImageCommand : IBotCommand
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ImageCommand(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task ExecuteAsync(BotCommandContext context)
    {
        using var client = _httpClientFactory.CreateClient("BotClient");
        await client.PostAsJsonAsync(
            "api/SendImageOverlay",
            new { resourceUrl = "https://cdn.pixabay.com/photo/2015/04/27/22/53/man-742766_960_720.jpg" });

        //if (context.Connection.State == HubConnectionState.Disconnected)
        //{
        //    await context.Connection.StartAsync();
        //}

        //await context.Connection.InvokeAsync(
        //    "SendOverlay",
        //    "https://cdn.pixabay.com/photo/2015/04/27/22/53/man-742766_960_720.jpg");
    }
}
