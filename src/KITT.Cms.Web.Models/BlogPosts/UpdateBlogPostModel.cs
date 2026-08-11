using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.BlogPosts;

public class UpdateBlogPostModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string PostAbstract { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTime? CreationDate { get; set; }

    [Required]
    public DateTime? PublicationDate { get; set; }

    public SeoData Seo { get; set; } = new();
}
