using KITT.Core.Validators;
using KITT.Telegram.Messages;

namespace KITT.Core.Commands;

public class StreamingCommands(KittDbContext context, StreamingValidator validator) : IStreamingCommands
{
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

        validator.ValidateForScheduleStreaming(streaming);

        context.Streamings.Add(streaming);
        await context.SaveChangesAsync();

        return streaming.Id;
    }

    public async Task UpdateStreamingAsync(
        Guid streamingId,
        string? twitchChannel,
        string? youTubeChannel,
        string streamingTitle, 
        DateOnly scheduleDate, 
        TimeOnly startingTime, 
        TimeOnly endingTime, 
        string twitchUrl, 
        string? streamingAbstract, 
        string youtubeUrl, 
        Content.SeoData seo)
    {
        var streaming = context.Streamings.SingleOrDefault(s => s.Id == streamingId);
        if (streaming is null)
        {
            throw new ArgumentOutOfRangeException(nameof(streamingId));
        }

        streaming.ChangeInformation(
            twitchChannel,
            youTubeChannel,
            streamingTitle,
            streamingAbstract,
            twitchUrl,
            youtubeUrl);

        streaming.ChangeSchedule(scheduleDate, startingTime, endingTime);

        if (seo is not null)
        {
            streaming.SetSeoData(seo);
        }

        validator.ValidateForUpdateStreaming(streaming);

        await context.SaveChangesAsync();
    }

    public async Task DeleteStreamingAsync(Guid streamingId)
    {
        var streaming = context.Streamings.SingleOrDefault(s => s.Id == streamingId);
        if (streaming is null)
        {
            throw new ArgumentOutOfRangeException(nameof(streamingId));
        }

        context.Streamings.Remove(streaming);
        await context.SaveChangesAsync();
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

        context.Streamings.Add(streaming);
        await context.SaveChangesAsync();

        return streaming.Id;
    }
}
