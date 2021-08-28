using System;
using System.Threading.Tasks;

namespace KITT.Core.Commands
{
    public interface IStreamingCommands
    {
        Task ScheduleStreamingAsync(string twitchChannel, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract);

        Task UpdateStreamingAsync(Guid streamingId, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink);

        Task RemoveStreamingAsync(Guid streamingId);
    }
}
