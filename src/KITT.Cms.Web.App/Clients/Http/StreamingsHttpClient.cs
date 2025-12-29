using KITT.Cms.Web.Models.Streamings;
using System.Net.Http.Json;

namespace KITT.Cms.Web.App.Clients.Http;

public class StreamingsHttpClient(HttpClient httpClient) : IStreamingsClient
{
    public string ApiResource { get; } = "/api/cms/streamings";

    public async Task ScheduleStreamingAsync(ScheduleStreamingModel model)
    {
        var response = await httpClient.PostAsJsonAsync(ApiResource, model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error scheduling streaming");
        }
    }

    public async Task<StreamingsListModel> GetAllStreamingsAsync(StreamingQueryModel query)
    {
        var url = ApiResource;
        var queryString = query.ToQueryString();
        if (!string.IsNullOrWhiteSpace(queryString))
        {
            url = $"{url}?{queryString}";
        }

        var model = await httpClient.GetFromJsonAsync<StreamingsListModel>(url);
        return model ?? new StreamingsListModel();
    }

    public async Task<StreamingDetailModel?> GetStreamingDetailAsync(Guid streamingId)
    {
        try
        {
            var model = await httpClient.GetFromJsonAsync<StreamingDetailModel>($"{ApiResource}/{streamingId}");
            return model;
        }
        catch
        {
            throw;
        }
    }

    public async Task UpdateStreamingAsync(StreamingDetailModel model)
    {
        var response = await httpClient.PutAsJsonAsync($"{ApiResource}/{model.Id}", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error updating streaming");
        }
    }

    public async Task DeleteStreamingAsync(Guid streamingId)
    {
        var response = await httpClient.DeleteAsync($"{ApiResource}/{streamingId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error deleting streaming");
        }
    }

    public async Task ImportStreamingAsync(ImportStreamingModel model)
    {
        var response = await httpClient.PostAsJsonAsync($"{ApiResource}/import", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Error importing streaming");
        }
    }

    public async Task<StreamingStatsModel> GetStreamingStatsAsync()
    {
        var streamingStats = await httpClient.GetFromJsonAsync<StreamingStatsModel>($"{ApiResource}/stats");
        return streamingStats ?? new(0, 0);
    }
}
