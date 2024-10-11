using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.Streamings;

public record StreamingDetailModel
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    [Required]
    public DateOnly ScheduleDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    public TimeOnly StartingTime { get; set; }

    [Required]
    public TimeOnly EndingTime { get; set; }

    [Required]
    public string HostingChannelUrl { get; set; } = string.Empty;

    public string? StreamingAbstract { get; set; }

    public string? YoutubeVideoUrl { get; set; }

    public SeoData Seo { get; set; } = new();
}
