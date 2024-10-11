using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.Streamings;

public record ImportStreamingModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public DateTime ScheduleDate { get; set; } = DateTime.Now;

    [Required]
    public TimeSpan StartingTime { get; set; }

    [Required]
    public TimeSpan EndingTime { get; set; }

    [Required]
    public string HostingChannelUrl { get; set; } = string.Empty;

    public string? StreamingAbstract { get; set; }

    public string? YoutubeVideoUrl { get; set; }

    public SeoData Seo { get; set; } = new();
}
