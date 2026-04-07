using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.Streamings;

public record ImportStreamingModel : IValidatableObject
{
    public string TwitchChannel { get; set; } = string.Empty;

    public string YouTubeChannel { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public DateOnly ScheduleDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    public TimeOnly StartingTime { get; set; }

    [Required]
    public TimeOnly EndingTime { get; set; }

    public string TwitchUrl { get; set; } = string.Empty;

    public string YouTubeUrl { get; set; } = string.Empty;

    public string? StreamingAbstract { get; set; }

    public SeoData Seo { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(TwitchChannel) && string.IsNullOrWhiteSpace(YouTubeChannel))
        {
            yield return new("At least a channel must be selected", [nameof(TwitchChannel), nameof(YouTubeChannel)]);
        }

        if (!string.IsNullOrWhiteSpace(TwitchChannel) && string.IsNullOrWhiteSpace(TwitchUrl))
        {
            yield return new("A Twitch URL is required since a Twitch channel has been selected", [nameof(TwitchUrl)]);
        }
        if (!string.IsNullOrWhiteSpace(YouTubeChannel) && string.IsNullOrWhiteSpace(YouTubeUrl))
        {
            yield return new("A YouTube URL is required since a YouTube channel has been selected", [nameof(YouTubeUrl)]);
        }
    }
}
