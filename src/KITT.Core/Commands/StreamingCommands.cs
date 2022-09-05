using KITT.Core.Validators;

namespace KITT.Core.Commands;

public class StreamingCommands : IStreamingCommands
{
    private readonly KittDbContext _context;

    private readonly StreamingValidator _validator;

    public StreamingCommands(KittDbContext context, StreamingValidator validator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Guid> ScheduleStreamingAsync(string userId, string twitchChannel, string streamingTitle, string streamingSlug, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, Content.SeoData seo)
    {
        var streaming = Streaming.Schedule(
            streamingTitle,
            streamingSlug,
            twitchChannel,
            scheduleDate,
            startingTime,
            endingTime,
            hostingChannelUrl,
            userId);

        if (!string.IsNullOrWhiteSpace(streamingAbstract))
        {
            streaming.SetAbstract(streamingAbstract);
        }

        if (seo is not null)
        {
            streaming.SetSeoData(seo);
        }

        _validator.ValidateForScheduleStreaming(streaming);

        _context.Streamings.Add(streaming);
        await _context.SaveChangesAsync();

        return streaming.Id;
    }

    public Task UpdateStreamingAsync(Guid streamingId, string streamingTitle, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink, Content.SeoData seo)
    {
        var streaming = _context.Streamings.SingleOrDefault(s => s.Id == streamingId);
        if (streaming is null)
        {
            throw new ArgumentOutOfRangeException(nameof(streamingId));
        }

        if (streaming.Title != streamingTitle)
        {
            streaming.ChangeTitle(streamingTitle);
        }

        if (ScheduleHasChanged(streaming, scheduleDate, startingTime, endingTime))
        {
            streaming.ChangeSchedule(scheduleDate, startingTime, endingTime);
        }

        if (streaming.HostingChannelUrl != hostingChannelUrl)
        {
            streaming.ChangeHostingChannelUrl(hostingChannelUrl);
        }

        if (streaming.Abstract != streamingAbstract)
        {
            streaming.SetAbstract(streamingAbstract);
        }

        if (streaming.YouTubeVideoUrl != youtubeRegistrationLink)
        {
            streaming.SetRegistrationYoutubeUrl(youtubeRegistrationLink);
        }

        if (seo is not null)
        {
            streaming.SetSeoData(seo);
        }

        _validator.ValidateForUpdateStreaming(streaming);

        return _context.SaveChangesAsync();
    }

    public Task DeleteStreamingAsync(Guid streamingId)
    {
        var streaming = _context.Streamings.SingleOrDefault(s => s.Id == streamingId);
        if (streaming is null)
        {
            throw new ArgumentOutOfRangeException(nameof(streamingId));
        }

        _context.Streamings.Remove(streaming);
        return _context.SaveChangesAsync();
    }

    public async Task<Guid> ImportStreamingAsync(string userId, string twitchChannel, string streamingTitle, string streamingSlug, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink, Content.SeoData seo)
    {
        var streaming = Streaming.Import(
            streamingTitle,
            streamingSlug,
            twitchChannel,
            scheduleDate,
            startingTime,
            endingTime,
            hostingChannelUrl,
            youtubeRegistrationLink,
            streamingAbstract,
            userId);

        if (seo is not null)
        {
            streaming.SetSeoData(seo);
        }

        _context.Streamings.Add(streaming);
        await _context.SaveChangesAsync();

        return streaming.Id;
    }

    #region Private methods
    private bool ScheduleHasChanged(Streaming streaming, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime)
        => streaming.ScheduleDate != scheduleDate || streaming.StartingTime != startingTime || streaming.EndingTime != endingTime;
    #endregion
}
