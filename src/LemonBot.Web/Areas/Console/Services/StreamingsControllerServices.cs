using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Web.Models.Streamings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LemonBot.Web.Areas.Console.Services
{
    public class StreamingsControllerServices
    {
        public IDatabase Database { get; }

        public IStreamingCommands Commands { get; }

        public StreamingsControllerServices(IDatabase database, IStreamingCommands commands)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public StreamingsListModel GetAllStreamings()
        {
            var streamings = Database.Streamings
                .Where(s => s.TwitchChannel == "albx87")
                .OrderByDescending(s => s.ScheduleDate)
                .ThenBy(s => s.StartingTime)
                .ThenBy(s => s.EndingTime)
                .Select(s => new StreamingsListModel.StreamingListItemModel
                {
                    Id = s.Id,
                    EndingTime = s.ScheduleDate.Add(s.EndingTime),
                    ScheduledOn = s.ScheduleDate,
                    StartingTime = s.ScheduleDate.Add(s.StartingTime),
                    Title = s.Title,
                    HostingChannelUrl = s.HostingChannelUrl,
                    YouTubeVideoUrl = s.YouTubeVideoUrl
                }).ToArray();

            var model = new StreamingsListModel { Items = streamings };
            return model;
        }

        public object GetStreamingDetail(Guid streamingId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> ScheduleStreamingAsync(ScheduleStreamingModel model, string userId)
        {
            return Commands.ScheduleStreamingAsync(
                userId,
                "albx87",
                model.Title,
                model.Slug,
                model.ScheduleDate,
                model.StartingTime.TimeOfDay,
                model.EndingTime.TimeOfDay,
                model.HostingChannelUrl,
                model.StreamingAbstract);
        }

        public Task UpdateStreamingAsync(Guid streamingId)
        {
            return Commands.UpdateStreamingAsync(
                streamingId,
                "",
                DateTime.Today,
                TimeSpan.FromHours(16),
                TimeSpan.FromHours(18),
                "",
                "",
                "");
        }

        public Task DeleteStreamingAsync(Guid streamingId) => Commands.DeleteStreamingAsync(streamingId);
    }
}
