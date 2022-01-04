using KITT.Web.Models.Streamings;
using System.Net.Http.Json;

namespace KITT.Web.App.Clients.Http;

public class StreamingsHttpClient : IStreamingsClient
{
    public HttpClient Client { get; }

    public string ApiResource { get; } = "/api/console/streamings";

    public StreamingsHttpClient(HttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task ScheduleStreamingAsync(ScheduleStreamingModel model)
    {
        var response = await Client.PostAsJsonAsync(ApiResource, model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error scheduling streaming");
        }
    }

    public async Task<StreamingsListModel> GetAllStreamingsAsync()
    {
        var model = await Client.GetFromJsonAsync<StreamingsListModel>(ApiResource);
        return model ?? new StreamingsListModel();
    }

    public async Task<StreamingDetailModel?> GetStreamingDetailAsync(Guid streamingId)
    {
        try
        {
            var model = await Client.GetFromJsonAsync<StreamingDetailModel>($"{ApiResource}/{streamingId}");
            return model;
        }
        catch 
        {
            throw;
        }
    }

    public async Task UpdateStreamingAsync(StreamingDetailModel model)
    {
        var response = await Client.PutAsJsonAsync($"{ApiResource}/{model.Id}", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error updating streaming");
        }
    }

    public async Task DeleteStreamingAsync(Guid streamingId)
    {
        var response = await Client.DeleteAsync($"{ApiResource}/{streamingId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error deleting streaming");
        }
    }

    public async Task ImportStreamingAsync(ImportStreamingModel model)
    {
        var response = await Client.PostAsJsonAsync($"{ApiResource}/import", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error importing streaming");
        }
    }
}
