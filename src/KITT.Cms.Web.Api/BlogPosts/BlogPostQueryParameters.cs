using KITT.Cms.Web.Models;
using KITT.Cms.Web.Models.BlogPosts;
using Microsoft.AspNetCore.Mvc;

namespace KITT.Cms.Web.Api.BlogPosts;

internal record BlogPostQueryParameters
{
    [FromQuery(Name = "q")]
    public string? Query { get; set; }

    [FromQuery]
    public ContentStatus? Status { get; set; }

    [FromQuery(Name = "sort")]
    public SortDirection PublishSort { get; set; } = SortDirection.Ascending;

    [FromQuery(Name = "p")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "s")]
    public int Size { get; set; } = 10;

    public static implicit operator BlogPostsQueryModel(BlogPostQueryParameters parameters) =>
        new()
        {
            Query = parameters.Query,
            Status = parameters.Status,
            PublishSort = parameters.PublishSort,
            Page = parameters.Page,
            Size = parameters.Size
        };
}
