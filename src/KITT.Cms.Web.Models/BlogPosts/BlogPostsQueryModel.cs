using Microsoft.AspNetCore.Mvc;

namespace KITT.Cms.Web.Models.BlogPosts;

public record BlogPostsQueryModel : QueryModel
{
    [FromQuery(Name = "q")]
    public string? Query { get; set; }

    [FromQuery(Name = "status")]
    public ContentStatus? Status { get; set; }

    [FromQuery(Name = "publishSort")]
    public SortDirection PublishSort { get; set; } = SortDirection.Ascending;

    [FromQuery(Name = "p")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "s")]
    public int Size { get; set; } = 10;
}
