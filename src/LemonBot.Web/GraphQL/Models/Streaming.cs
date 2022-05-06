namespace LemonBot.Web.GraphQL.Models;

public class Streaming
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Slug { get; init; } = string.Empty;

    public string Abstract { get; init; } = string.Empty;

    public DateTime ScheduleDate { get; init; }

    public TimeSpan StartingTime { get; init; }

    public TimeSpan EndingTime { get; init; }

    public string HostingChannelUrl { get; init; } = string.Empty;

    public string YouTubeVideoUrl { get; init; } = string.Empty;

    public SeoInfo Seo { get; init; } = default!;
}
