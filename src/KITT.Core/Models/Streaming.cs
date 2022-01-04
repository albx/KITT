namespace KITT.Core.Models;

public class Streaming : Content
{
    public string TwitchChannel { get; protected set; }

    public DateTime ScheduleDate { get; protected set; }

    public TimeSpan StartingTime { get; protected set; }

    public TimeSpan EndingTime { get; protected set; }

    public string HostingChannelUrl { get; protected set; }

    public string YouTubeVideoUrl { get; protected set; }

    #region Constructor
    protected Streaming() : base() { }
    #endregion

    #region Behaviors
    public void ChangeSchedule(DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime)
    {
        if (scheduleDate < DateTime.Today)
        {
            throw new ArgumentException("Schedule date cannot be set in the past", nameof(scheduleDate));
        }

        if (startingTime >= endingTime)
        {
            throw new ArgumentException("Ending time cannot be previous than starting time", nameof(endingTime));
        }

        this.ScheduleDate = scheduleDate;
        this.StartingTime = startingTime;
        this.EndingTime = endingTime;
    }

    public void SetRegistrationYoutubeUrl(string youtubeUrl)
    {
        this.YouTubeVideoUrl = youtubeUrl;
    }

    public void ChangeHostingChannelUrl(string hostingChannelUrl)
    {
        this.HostingChannelUrl = hostingChannelUrl;
    }
    #endregion

    #region Factory
    public static Streaming Schedule(string title, string slug, string twitchChannel, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string userId)
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

        if (scheduleDate < DateTime.Today)
        {
            throw new ArgumentException("Schedule date cannot be set in the past", nameof(scheduleDate));
        }

        if (startingTime >= endingTime)
        {
            throw new ArgumentException("Starting time should be previuos than ending time", nameof(endingTime));
        }

        if (string.IsNullOrWhiteSpace(hostingChannelUrl))
        {
            throw new ArgumentException("value cannot be empty", nameof(hostingChannelUrl));
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
            HostingChannelUrl = hostingChannelUrl
        };

        streaming.Publish();

        return streaming;
    }

    public static Streaming Import(string title, string slug, string twitchChannel, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string youtubeVideoUrl, string @abstract, string userId)
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

        if (scheduleDate < DateTime.Today)
        {
            throw new ArgumentException("Schedule date cannot be set in the past", nameof(scheduleDate));
        }

        if (startingTime >= endingTime)
        {
            throw new ArgumentException("Starting time should be previuos than ending time", nameof(endingTime));
        }

        if (string.IsNullOrWhiteSpace(hostingChannelUrl))
        {
            throw new ArgumentException("value cannot be empty", nameof(hostingChannelUrl));
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
            HostingChannelUrl = hostingChannelUrl,
            Abstract = @abstract,
            YouTubeVideoUrl = youtubeVideoUrl
        };

        streaming.Publish();

        return streaming;
    }
    #endregion
}
