namespace KITT.Core.Commands;

public interface IStreamingCommands
{
    Task<Guid> ScheduleStreamingAsync(string userId, string twitchChannel, string streamingTitle, string streamingSlug, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract);

    Task UpdateStreamingAsync(Guid streamingId, string streamingTitle, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink);

    Task DeleteStreamingAsync(Guid streamingId);

    Task<Guid> ImportStreamingAsync(string userId, string twitchChannel, string streamingTitle, string streamingSlug, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink);
}
