using KITT.Web.Models.Tools;
using System.Net.Http.Json;

namespace KITT.Web.App.Tools.Clients.Http;

public class StreamingsHttpClient : IStreamingsClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "/api/tools/streamings";

    public StreamingsHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<IEnumerable<ScheduledStreamingModel>> GetScheduledStreamingsAsync()
    {
        var scheduledStreamings = await Client.GetFromJsonAsync<IEnumerable<ScheduledStreamingModel>>($"{ApiResource}/scheduled");
        return scheduledStreamings ?? Array.Empty<ScheduledStreamingModel>();
    }
}
