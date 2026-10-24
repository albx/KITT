using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.BlogPosts;

public class DraftBlogPostModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string PostAbstract { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public SeoData Seo { get; set; } = new();
}
