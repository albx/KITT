namespace KITT.Cms.Web.Models.BlogPosts;

public record BlogPostsQueryModel : QueryModel
{
    public string? Query { get; set; }

    public ContentStatus? Status { get; set; }

    public SortDirection PublishSort { get; set; } = SortDirection.Ascending;

    public int Page { get; set; } = 1;

    public int Size { get; set; } = 10;
}
