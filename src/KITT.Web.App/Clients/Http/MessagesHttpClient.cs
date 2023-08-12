using KITT.Web.Models.Messages;
using System.Net.Http.Json;

namespace KITT.Web.App.Clients.Http;

public class MessagesHttpClient : IMessagesClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "/api/messages";

    public MessagesHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task SendMessageAsync(SendMessageModel message)
    {
        await Client.PostAsJsonAsync(ApiResource, message);
    }
}
