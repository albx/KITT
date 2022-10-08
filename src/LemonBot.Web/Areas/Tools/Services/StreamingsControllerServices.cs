using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Web.Models.Tools;

namespace LemonBot.Web.Areas.Tools.Services;

public class StreamingsControllerServices
{
    public IDatabase Database { get; }
    public IStreamingStatsCommands Commands { get; }

    public StreamingsControllerServices(IDatabase database, IStreamingStatsCommands commands)
    {
        Database = database ?? throw new ArgumentNullException(nameof(database));
        Commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    public IEnumerable<ScheduledStreamingModel> GetScheduledStreamings(string userId)
    {
        var scheduledStreamings = Database.Streamings
            .ByUserId(userId)
            .Where(s => s.ScheduleDate >= DateTime.Today)
            .OrderedBySchedule()
            .Select(s => new ScheduledStreamingModel
            {
                Id = s.Id,
                Title = s.Title,
                ScheduleDate = s.ScheduleDate
            }).ToArray();

        return scheduledStreamings;
    }

    public Task SaveStreamingStatsAsync(Guid streamingId, StreamingStats model)
    {
        return Commands.RegisterStreamingStatsAsync(
            streamingId,
            model.Viewers,
            model.Subscribers,
            model.UserJoinedNumber,
            model.UserLeftNumber);
    }
}
