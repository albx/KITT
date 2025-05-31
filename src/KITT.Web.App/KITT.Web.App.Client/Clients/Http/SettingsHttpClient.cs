using KITT.Cms.Web.Models.Settings;
using System.Net.Http.Json;

namespace KITT.Web.App.Client.Clients.Http;

public class SettingsHttpClient : ISettingsClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "/api/cms/settings";

    public SettingsHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task CreateNewSettingsAsync(CreateNewSettingsModel model)
    {
        var response = await Client.PostAsJsonAsync(ApiResource, model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error creating settings");
        }
    }

    public async Task<SettingsListModel> GetAllSettingsAsync()
    {
        var model = await Client.GetFromJsonAsync<SettingsListModel>(ApiResource);
        return model ?? new();
    }
}
