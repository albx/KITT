using KITT.Core.Commands;
using KITT.Core.ReadModels;
using KITT.Web.Models.Streamings;
using Microsoft.EntityFrameworkCore;

namespace LemonBot.Web.Endpoints.Services;

public class StreamingsEndpointsServices
{
    public IDatabase Database { get; }

    public IStreamingCommands Commands { get; }

    public StreamingsEndpointsServices(IDatabase database, IStreamingCommands commands)
    {
        Database = database ?? throw new ArgumentNullException(nameof(database));
        Commands = commands ?? throw new ArgumentNullException(nameof(commands));
    }

    public async Task<StreamingsListModel> GetAllStreamingsAsync(string userId, int page, int size, StreamingQueryModel.SortDirection sort, string? query)
    {
        var ascending = sort == StreamingQueryModel.SortDirection.Ascending;

        var streamingsQuery = Database.Streamings
            .ByUserId(userId)
            .OrderedBySchedule(ascending);

        if (!string.IsNullOrWhiteSpace(query))
        {
            streamingsQuery = streamingsQuery.Where(s => s.Title.Contains(query));
        }

        var skip = (page - 1) * size;

        var streamings = await streamingsQuery
            .Select(s => new StreamingsListModel.StreamingListItemModel
            {
                Id = s.Id,
                EndingTime = s.EndingTime,
                ScheduledOn = s.ScheduleDate,
                StartingTime = s.StartingTime,
                Title = s.Title,
                HostingChannelUrl = s.HostingChannelUrl,
                YouTubeVideoUrl = s.YouTubeVideoUrl
            }).Skip(skip).Take(size).ToArrayAsync();

        var model = new StreamingsListModel { TotalItems = streamingsQuery.Count(), Items = streamings };
        return model;
    }

    public async Task<StreamingDetailModel?> GetStreamingDetailAsync(Guid streamingId)
    {
        var streaming = await Database.Streamings.SingleOrDefaultAsync(s => s.Id == streamingId);
        if (streaming is null)
        {
            return null;
        }

        return new()
        {
            Id = streaming.Id,
            ScheduleDate = streaming.ScheduleDate,
            EndingTime = streaming.EndingTime,
            HostingChannelUrl = streaming.HostingChannelUrl,
            StartingTime = streaming.StartingTime,
            StreamingAbstract = streaming.Abstract,
            Title = streaming.Title,
            YoutubeVideoUrl = streaming.YouTubeVideoUrl,
            Slug = streaming.Slug
        };
    }

    public Task<Guid> ImportStreamingAsync(ImportStreamingModel model, string userId)
    {
        var settings = Database.Settings
            .ByUserId(userId)
            .FirstOrDefault();

        if (settings is null)
        {
            throw new InvalidOperationException("No settings configured");
        }

        var seo = new KITT.Core.Models.Content.SeoData
        {
            Title = model.Seo.Title,
            Description = model.Seo.Description,
            Keywords = model.Seo.Keywords
        };

        return Commands.ImportStreamingAsync(
            userId,
            settings.TwitchChannel,
            model.Title,
            model.Slug,
            DateOnly.FromDateTime(model.ScheduleDate),
            TimeOnly.FromTimeSpan(model.StartingTime),
            TimeOnly.FromTimeSpan(model.EndingTime),
            model.HostingChannelUrl,
            model.StreamingAbstract,
            model.YoutubeVideoUrl,
            seo);
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

        var seo = new KITT.Core.Models.Content.SeoData
        {
            Title = model.Seo.Title,
            Description = model.Seo.Description,
            Keywords = model.Seo.Keywords
        };

        return Commands.ScheduleStreamingAsync(
            userId,
            settings.TwitchChannel,
            model.Title,
            model.Slug,
            model.ScheduleDate,
            model.StartingTime,
            model.EndingTime,
            model.HostingChannelUrl,
            model.StreamingAbstract,
            seo);
    }

    public Task UpdateStreamingAsync(Guid streamingId, StreamingDetailModel model)
    {
        var seo = new KITT.Core.Models.Content.SeoData
        {
            Title = model.Seo.Title,
            Description = model.Seo.Description,
            Keywords = model.Seo.Keywords
        };

        return Commands.UpdateStreamingAsync(
            streamingId,
            model.Title,
            model.ScheduleDate,
            model.StartingTime,
            model.EndingTime,
            model.HostingChannelUrl,
            model.StreamingAbstract,
            model.YoutubeVideoUrl,
            seo);
    }

    public Task DeleteStreamingAsync(Guid streamingId) => Commands.DeleteStreamingAsync(streamingId);
}
