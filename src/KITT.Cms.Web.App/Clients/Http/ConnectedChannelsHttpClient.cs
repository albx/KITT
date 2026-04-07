using KITT.Cms.Web.Models.Settings;
using System.Net.Http.Json;

namespace KITT.Cms.Web.App.Clients.Http;

public class ConnectedChannelsHttpClient(HttpClient httpClient) : IConnectedChannelsClient
{
    public string ApiResource { get; } = "/api/cms/settings/channels";

    public async Task<ChannelModel> CreateNewConnectedChannelAsync(ChannelModel model)
    {
        var response = await httpClient.PostAsJsonAsync(ApiResource, model);
        response.EnsureSuccessStatusCode();

        var createdChannel = await response.Content.ReadFromJsonAsync<ChannelModel>();
        return createdChannel ?? throw new InvalidOperationException("Invalid channel");
    }

    public async Task DeleteConnectedChannelAsync(ChannelModel model)
    {
        var response = await httpClient.DeleteAsync($"{ApiResource}/{model.Id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<ChannelModel[]> GetConnectedChannelsAsync()
    {
        var channels = await httpClient.GetFromJsonAsync<ChannelModel[]>(ApiResource);
        return channels ?? [];
    }

    public async Task UpdateConnectedChannelAsync(ChannelModel model)
    {
        var response = await httpClient.PutAsJsonAsync($"{ApiResource}/{model.Id}", model);
        response.EnsureSuccessStatusCode();
    }
}
