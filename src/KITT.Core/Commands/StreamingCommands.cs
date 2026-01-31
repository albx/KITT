using KITT.Core.Validators;
using KITT.Telegram.Messages;

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

    public async Task<Guid> ScheduleStreamingAsync(
        string userId, 
        string twitchChannel, 
        string youTubeChannel,
        string streamingTitle, 
        string streamingSlug, 
        DateOnly scheduleDate, 
        TimeOnly startingTime, 
        TimeOnly endingTime, 
        string twitchUrl, 
        string youTubeUrl,
        string? streamingAbstract, 
        Content.SeoData seo)
    {
        var streaming = Streaming.Schedule(
            streamingTitle,
            streamingSlug,
            twitchChannel,
            youTubeChannel,
            scheduleDate,
            startingTime,
            endingTime,
            twitchUrl,
            youTubeUrl,
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

    public async Task UpdateStreamingAsync(Guid streamingId, string streamingTitle, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string twitchUrl, string? streamingAbstract, string youtubeUrl, Content.SeoData seo)
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
        }

        if (streaming.TwitchUrl != twitchUrl)
        {
            streaming.SetTwitchUrl(twitchUrl);
        }

        if (streaming.Abstract != streamingAbstract)
        {
            streaming.SetAbstract(streamingAbstract);
        }

        if (streaming.YouTubeUrl != youtubeUrl)
        {
            streaming.SetYoutubeUrl(youtubeUrl);
        }

        if (seo is not null)
        {
            streaming.SetSeoData(seo);
        }

        _validator.ValidateForUpdateStreaming(streaming);

        await _context.SaveChangesAsync();
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
    }

    public async Task<Guid> ImportStreamingAsync(
        string userId, 
        string twitchChannel, 
        string youTubeChannel,
        string streamingTitle, 
        string streamingSlug, 
        DateOnly scheduleDate, 
        TimeOnly startingTime, 
        TimeOnly endingTime, 
        string twitchUrl, 
        string? streamingAbstract, 
        string? youTubeUrl, 
        Content.SeoData seo)
    {
        var streaming = Streaming.Import(
            streamingTitle,
            streamingSlug,
            twitchChannel,
            youTubeChannel,
            scheduleDate,
            startingTime,
            endingTime,
            twitchUrl,
            youTubeUrl,
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
    private bool ScheduleHasChanged(Streaming streaming, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime)
        => streaming.ScheduleDate != scheduleDate || streaming.StartingTime != startingTime || streaming.EndingTime != endingTime;
    #endregion
}
