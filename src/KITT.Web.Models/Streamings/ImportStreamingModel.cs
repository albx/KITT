using System.ComponentModel.DataAnnotations;

namespace KITT.Web.Models.Streamings;

public class ImportStreamingModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public DateTime ScheduleDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime StartingTime { get; set; }

    [Required]
    public DateTime EndingTime { get; set; }

    [Required]
    public string HostingChannelUrl { get; set; } = string.Empty;

    public string? StreamingAbstract { get; set; }

    public string? YoutubeVideoUrl { get; set; }
}
