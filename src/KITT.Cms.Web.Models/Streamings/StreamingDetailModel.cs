using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.Streamings;

public record StreamingDetailModel
{
    public Guid Id { get; set; }

    public string? TwitchChannel { get; set; }

    public string? YouTubeChannel { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    [Required]
    public DateOnly ScheduleDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    public TimeOnly StartingTime { get; set; }

    [Required]
    public TimeOnly EndingTime { get; set; }

    public string TwitchUrl { get; set; } = string.Empty;

    public string? StreamingAbstract { get; set; }

    public string YouTubeUrl { get; set; } = string.Empty;

    public SeoData Seo { get; set; } = new();
}
