using KITT.Cms.Web.Models.Contents;
using OperationResults;
using System.Net.Http.Json;

namespace KITT.Cms.Web.App.Clients.Http;

public class ContentHttpClient(HttpClient httpClient) : IContentClient
{
    public string ApiResource { get; } = "/api/cms/contents";

    public async Task<Result> PublishContentAsync(Guid contentId, PublishContentModel model)
    {
        var response = await httpClient.PatchAsJsonAsync($"{ApiResource}/{contentId}/publish", model);

        return response.IsSuccessStatusCode
            ? Result.Ok()
            : Result.Fail(FailureReasons.ClientError);
    }
}
