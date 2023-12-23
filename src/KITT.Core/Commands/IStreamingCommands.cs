namespace KITT.Core.Commands;

public interface IStreamingCommands
{
    Task<Guid> ScheduleStreamingAsync(string userId, string twitchChannel, string streamingTitle, string streamingSlug, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string hostingChannelUrl, string streamingAbstract, Content.SeoData seo);

    Task UpdateStreamingAsync(Guid streamingId, string streamingTitle, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink, Content.SeoData seo);

    Task DeleteStreamingAsync(Guid streamingId);

    Task<Guid> ImportStreamingAsync(string userId, string twitchChannel, string streamingTitle, string streamingSlug, DateOnly scheduleDate, TimeOnly startingTime, TimeOnly endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink, Content.SeoData seo);
}
