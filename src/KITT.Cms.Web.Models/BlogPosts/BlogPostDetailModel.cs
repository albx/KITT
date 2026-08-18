using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.BlogPosts;

public class BlogPostDetailModel
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string? PostAbstract { get; set; }

    public string Content { get; set; } = string.Empty;

    public SeoData Seo { get; set; } = new();

    public DateTime CreationDate { get; set; }

    public DateTime? PublicationDate { get; set; }
}
