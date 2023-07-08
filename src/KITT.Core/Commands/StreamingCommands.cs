using KITT.Core.Validators;
using KITT.Telegram.Messages;
using KITT.Telegram.Messages.Streaming;

namespace KITT.Core.Commands;

public class StreamingCommands : IStreamingCommands
{
    private readonly KittDbContext _context;

    private readonly StreamingValidator _validator;

    private readonly IMessageBus _messageBus;

    public StreamingCommands(KittDbContext context, StreamingValidator validator, IMessageBus messageBus)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
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

        var message = new StreamingScheduledMessage(
            streaming.Id,
            streamingTitle,
            streamingSlug,
            DateOnly.FromDateTime(scheduleDate),
            TimeOnly.FromTimeSpan(startingTime),
            TimeOnly.FromTimeSpan(endingTime),
            hostingChannelUrl);

        await _messageBus.SendAsync(message);

        return streaming.Id;
    }

    public async Task UpdateStreamingAsync(Guid streamingId, string streamingTitle, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink, Content.SeoData seo)
    {
        var messagesToSend = new List<object>();

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
            var scheduledChangedMessage = new StreamingScheduleChangedMessage(
                streamingId,
                streaming.Title,
                streaming.Slug,
                DateOnly.FromDateTime(scheduleDate),
                TimeOnly.FromTimeSpan(startingTime),
                TimeOnly.FromTimeSpan(endingTime));

            messagesToSend.Add(scheduledChangedMessage);
        }

        if (streaming.HostingChannelUrl != hostingChannelUrl)
        {
            streaming.ChangeHostingChannelUrl(hostingChannelUrl);

            var hostingChannelChangedMessage = new StreamingHostingChannelChangedMessage(
                streamingId,
                streaming.Title,
                streaming.Slug,
                hostingChannelUrl,
                DateOnly.FromDateTime(scheduleDate),
                TimeOnly.FromTimeSpan(startingTime));

            messagesToSend.Add(hostingChannelChangedMessage);
        }

        if (streaming.Abstract != streamingAbstract)
        {
            streaming.SetAbstract(streamingAbstract);
        }

        if (streaming.YouTubeVideoUrl != youtubeRegistrationLink)
        {
            streaming.SetRegistrationYoutubeUrl(youtubeRegistrationLink);
            var videoUploadedMessage = new StreamingVideoUploadedMessage(
                streamingId,
                streaming.Title,
                streaming.Slug,
                youtubeRegistrationLink);

            messagesToSend.Add(videoUploadedMessage);
        }

        if (seo is not null)
        {
            streaming.SetSeoData(seo);
        }

        _validator.ValidateForUpdateStreaming(streaming);

        await _context.SaveChangesAsync();

        foreach (var message in messagesToSend)
        {
            await _messageBus.SendAsync(message);
        }
    }

    public async Task DeleteStreamingAsync(Guid streamingId)
    {
        var streaming = _context.Streamings.SingleOrDefault(s => s.Id == streamingId);
        if (streaming is null)
        {
            throw new ArgumentOutOfRangeException(nameof(streamingId));
        }

        _context.Streamings.Remove(streaming);
        await _context.SaveChangesAsync();

        var message = new StreamingCanceledMessage(
            streamingId,
            streaming.Title,
            DateOnly.FromDateTime(streaming.ScheduleDate),
            TimeOnly.FromTimeSpan(streaming.StartingTime),
            TimeOnly.FromTimeSpan(streaming.EndingTime));

        await _messageBus.SendAsync(message);
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
