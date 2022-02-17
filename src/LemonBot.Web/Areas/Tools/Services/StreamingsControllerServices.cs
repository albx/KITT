using KITT.Core.ReadModels;
using KITT.Web.Models.Tools;

namespace LemonBot.Web.Areas.Tools.Services;

public class StreamingsControllerServices
{
    public IDatabase Database { get; }

    public StreamingsControllerServices(IDatabase database)
    {
        Database = database ?? throw new ArgumentNullException(nameof(database));
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
                Title = s.Title
            }).ToArray();

        return scheduledStreamings;
    }
}
