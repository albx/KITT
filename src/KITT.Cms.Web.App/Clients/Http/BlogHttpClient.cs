using KITT.Cms.Web.Models.BlogPosts;
using OperationResults;
using System.Net;
using System.Net.Http.Json;

namespace KITT.Cms.Web.App.Clients.Http;

public class BlogHttpClient(HttpClient httpClient) : IBlogClient
{
    public string ApiResource { get; } = "/api/cms/blogposts";

    public async Task<Result<BlogPostDetailModel>> GetPostDetailAsync(Guid postId)
    {
        try
        {
            var post = await httpClient.GetFromJsonAsync<BlogPostDetailModel>($"{ApiResource}/{postId}");
            return post!;
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.NotFound => Result.Fail(FailureReasons.ItemNotFound),
                _ => Result.Fail(FailureReasons.GenericError)
            };
        }
    }

    public async Task<Result<BlogPostDetailModel>> ImportPostAsync(ImportBlogPostModel model)
    {
        var response = await httpClient.PostAsJsonAsync($"{ApiResource}/import", model);
        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        var post = await response.Content.ReadFromJsonAsync<BlogPostDetailModel>();
        return post!;
    }

    public async Task<Result<BlogPostDetailModel>> SavePostDraftAsync(DraftBlogPostModel model)
    {
        var response = await httpClient.PostAsJsonAsync($"{ApiResource}/draft", model);
        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        var post = await response.Content.ReadFromJsonAsync<BlogPostDetailModel>();
        return post!;
    }
}
