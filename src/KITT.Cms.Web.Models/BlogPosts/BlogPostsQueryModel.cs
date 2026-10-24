using System.Web;

namespace KITT.Cms.Web.Models.BlogPosts;

public record BlogPostsQueryModel : QueryModel
{
    public string? Query { get; set; }

    public ContentStatus? Status { get; set; }

    public SortDirection PublishSort { get; set; } = SortDirection.Ascending;

    public int Page { get; set; } = 1;

    public int Size { get; set; } = 10;

    public string ToQueryString()
    {
        var queryItems = new List<string>
        {
            $"p={Page}",
            $"s={Size}",
            $"sort={PublishSort}"
        };

        if (Status.HasValue)
        {
            queryItems.Add($"status={Status.Value}");
        }

        if (!string.IsNullOrWhiteSpace(Query))
        {
            queryItems.Add($"q={HttpUtility.UrlEncode(Query)}");
        }

        return string.Join("&", queryItems);
    }
}
