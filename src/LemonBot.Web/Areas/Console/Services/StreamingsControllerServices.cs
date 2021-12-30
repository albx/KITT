using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Web.Models.Streamings;

namespace LemonBot.Web.Areas.Console.Services;

public class StreamingsControllerServices
{
    public IDatabase Database { get; }

    public IStreamingCommands Commands { get; }

    public StreamingsControllerServices(IDatabase database, IStreamingCommands commands)
    {
        Database = database ?? throw new ArgumentNullException(nameof(database));
        Commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    public StreamingsListModel GetAllStreamings(string userId)
    {
        var streamings = Database.Streamings
            .ByUserId(userId)
            .OrderedBySchedule()
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

    public StreamingDetailModel? GetStreamingDetail(Guid streamingId)
    {
        var streaming = Database.Streamings.SingleOrDefault(s => s.Id == streamingId);
        if (streaming is null)
        {
            return null;
        }

        return new()
        {
            Id = streaming.Id,
            ScheduleDate = streaming.ScheduleDate,
            EndingTime = streaming.ScheduleDate.Add(streaming.EndingTime),
            HostingChannelUrl = streaming.HostingChannelUrl,
            StartingTime = streaming.ScheduleDate.Add(streaming.StartingTime),
            StreamingAbstract = streaming.Abstract,
            Title = streaming.Title,
            YoutubeVideoUrl = streaming.YouTubeVideoUrl,
            Slug = streaming.Slug
        };
    }

    public Task<Guid> ScheduleStreamingAsync(ScheduleStreamingModel model, string userId)
    {
        var settings = Database.Settings
            .ByUserId(userId)
            .FirstOrDefault();

        if (settings is null)
        {
            throw new InvalidOperationException("No settings configured");
        }

        return Commands.ScheduleStreamingAsync(
            userId,
            settings.TwitchChannel,
            model.Title,
            model.Slug,
            model.ScheduleDate,
            model.StartingTime.TimeOfDay,
            model.EndingTime.TimeOfDay,
            model.HostingChannelUrl,
            model.StreamingAbstract);
    }

    public Task UpdateStreamingAsync(Guid streamingId, StreamingDetailModel model)
    {
        return Commands.UpdateStreamingAsync(
            streamingId,
            model.Title,
            model.ScheduleDate,
            model.StartingTime.TimeOfDay,
            model.EndingTime.TimeOfDay,
            model.HostingChannelUrl,
            model.StreamingAbstract,
            model.YoutubeVideoUrl);
    }

    public Task DeleteStreamingAsync(Guid streamingId) => Commands.DeleteStreamingAsync(streamingId);
}
