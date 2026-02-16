namespace KITT.Core.Commands;

public interface IStreamingCommands
{
    Task<Guid> ScheduleStreamingAsync(
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
        Content.SeoData seo);

    Task UpdateStreamingAsync(
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
        Content.SeoData seo);

    Task DeleteStreamingAsync(Guid streamingId);

    Task<Guid> ImportStreamingAsync(
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
        Content.SeoData seo);
}
