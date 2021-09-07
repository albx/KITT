using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Web.Models.Lives;
using System;
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

        public object GetAllStreamings()
        {
            throw new NotImplementedException();
        }

        public object GetStreamingDetail(Guid streamingId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> ScheduleStreamingAsync(ScheduleStreamingModel model)
        {
            return Commands.ScheduleStreamingAsync(
                model.TwitchChannelUrl,
                model.Title,
                "",
                model.ScheduleDate,
                model.StartingTime,
                model.EndingTime,
                "",
                "");
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
