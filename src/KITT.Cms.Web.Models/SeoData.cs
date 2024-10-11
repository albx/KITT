namespace KITT.Cms.Web.Models;

public record SeoData
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;
}
