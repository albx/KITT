namespace KITT.Core.Models;

public class Streaming : Content
{
    public string? TwitchChannel { get; protected set; }

    public string? YouTubeChannel { get; protected set; }

    public DateOnly ScheduleDate { get; protected set; }

    public TimeOnly StartingTime { get; protected set; }

    public TimeOnly EndingTime { get; protected set; }

    public string? TwitchUrl { get; protected set; }

    public string? YouTubeUrl { get; protected set; }

    #region Constructor
    protected Streaming() : base() { }
    #endregion

    #region Behaviors
    public void ChangeSchedule(DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime)
    {
        if (startingTime >= endingTime)
        {
            throw new ArgumentException("Ending time cannot be previous than starting time", nameof(endingTime));
        }

        this.ScheduleDate = scheduleDate;
        this.StartingTime = startingTime;
        this.EndingTime = endingTime;
    }

    public void SetYoutubeUrl(string youtubeUrl)
    {
        YouTubeUrl = youtubeUrl;
    }

    public void SetTwitchUrl(string twitchUrl)
    {
        TwitchUrl = twitchUrl;
    }
    #endregion

    #region Factory
    public static Streaming Schedule(string title, string slug, string twitchChannel, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string twitchUrl, string userId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("value cannot be empty", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("value cannot be empty", nameof(slug));
        }

        if (string.IsNullOrWhiteSpace(twitchChannel))
        {
            throw new ArgumentException("value cannot be empty", nameof(twitchChannel));
        }

        if (scheduleDate < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ArgumentException("Schedule date cannot be set in the past", nameof(scheduleDate));
        }

        if (startingTime >= endingTime)
        {
            throw new ArgumentException("Starting time should be previuos than ending time", nameof(endingTime));
        }

        if (string.IsNullOrWhiteSpace(twitchUrl))
        {
            throw new ArgumentException("value cannot be empty", nameof(twitchUrl));
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("value cannot be empty", nameof(userId));
        }

        var streaming = new Streaming
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Slug = slug,
            TwitchChannel = twitchChannel,
            ScheduleDate = scheduleDate,
            StartingTime = startingTime,
            EndingTime = endingTime,
            TwitchUrl = twitchUrl
        };

        streaming.Publish();

        return streaming;
    }

    public static Streaming Import(string title, string slug, string twitchChannel, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string twitchUrl, string? youtubeUrl, string? @abstract, string userId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("value cannot be empty", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("value cannot be empty", nameof(slug));
        }

        if (string.IsNullOrWhiteSpace(twitchChannel))
        {
            throw new ArgumentException("value cannot be empty", nameof(twitchChannel));
        }

        if (startingTime >= endingTime)
        {
            throw new ArgumentException("Starting time should be previuos than ending time", nameof(endingTime));
        }

        if (string.IsNullOrWhiteSpace(twitchUrl))
        {
            throw new ArgumentException("value cannot be empty", nameof(twitchUrl));
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("value cannot be empty", nameof(userId));
        }

        var streaming = new Streaming
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Slug = slug,
            TwitchChannel = twitchChannel,
            ScheduleDate = scheduleDate,
            StartingTime = startingTime,
            EndingTime = endingTime,
            TwitchUrl = twitchUrl,
            Abstract = @abstract,
            YouTubeUrl = youtubeUrl
        };

        streaming.PublishOn(scheduleDate.ToDateTime(TimeOnly.MinValue));

        return streaming;
    }
    #endregion
}
