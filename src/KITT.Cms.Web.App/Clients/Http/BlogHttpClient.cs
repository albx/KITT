namespace KITT.Cms.Web.App.Clients.Http;

public class BlogHttpClient(HttpClient httpClient) : IBlogClient
{
    public string ApiResource { get; } = "/api/cms/blogposts";
}
