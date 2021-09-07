using KITT.Core.Models;
using KITT.Core.Persistence;
using KITT.Core.Validators;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KITT.Core.Commands
{
    public class StreamingCommands : IStreamingCommands
    {
        private readonly KittDbContext _context;

        private readonly StreamingValidator _validator;

        public StreamingCommands(KittDbContext context, StreamingValidator validator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<Guid> ScheduleStreamingAsync(string twitchChannel, string streamingTitle, string streamingSlug, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract)
        {
            var streaming = Streaming.Schedule(
                streamingTitle,
                streamingSlug,
                twitchChannel,
                scheduleDate,
                startingTime,
                endingTime,
                hostingChannelUrl);

            if (!string.IsNullOrWhiteSpace(streamingAbstract))
            {
                streaming.SetAbstract(streamingAbstract);
            }

            _validator.ValidateForScheduleStreaming(streaming);

            _context.Streamings.Add(streaming);
            await _context.SaveChangesAsync();

            return streaming.Id;
        }

        public Task UpdateStreamingAsync(Guid streamingId, string streamingTitle, DateTime scheduleDate, TimeSpan startingTime, TimeSpan endingTime, string hostingChannelUrl, string streamingAbstract, string youtubeRegistrationLink)
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

            streaming.ChangeSchedule(scheduleDate, startingTime, endingTime);

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
    }
}
