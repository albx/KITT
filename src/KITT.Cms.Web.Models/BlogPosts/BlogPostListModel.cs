namespace KITT.Cms.Web.Models.BlogPosts;

public class BlogPostListModel
{
    public int TotalItems { get; set; }

    public IEnumerable<BlogPostListItemModel> Items { get; set; } = [];

    public record BlogPostListItemModel
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;
        
        public string Slug { get; init; } = string.Empty;
        
        public DateTime? PublishedOn { get; init; }

        public ContentStatus Status { get; init; }
    }
}
