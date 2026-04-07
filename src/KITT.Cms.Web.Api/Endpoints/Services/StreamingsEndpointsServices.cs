using KITT.Cms.Web.Models.Streamings;
using KITT.Core.Commands;
using KITT.Core.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace KITT.Cms.Web.Api.Endpoints.Services;

public class StreamingsEndpointsServices(IDatabase database, IStreamingCommands commands)
{
    public IDatabase Database { get; } = database ?? throw new ArgumentNullException(nameof(database));

    public IStreamingCommands Commands { get; } = commands ?? throw new ArgumentNullException(nameof(commands));

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
                HostingChannelUrl = s.TwitchUrl ?? string.Empty,
                YouTubeVideoUrl = s.YouTubeUrl
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
            TwitchChannel = streaming.TwitchChannel,
            YouTubeChannel = streaming.YouTubeChannel,
            ScheduleDate = streaming.ScheduleDate,
            EndingTime = streaming.EndingTime,
            TwitchUrl = streaming.TwitchUrl ?? string.Empty,
            StartingTime = streaming.StartingTime,
            StreamingAbstract = streaming.Abstract,
            Title = streaming.Title,
            YouTubeUrl = streaming.YouTubeUrl ?? string.Empty,
            Slug = streaming.Slug,
            Seo = new Models.SeoData 
            { 
                Title = streaming.Seo?.Title ?? string.Empty, 
                Description = streaming.Seo?.Description ?? string.Empty,
                Keywords = streaming.Seo?.Keywords ?? string.Empty,
            }
        };
    }

    public Task<Guid> ImportStreamingAsync(ImportStreamingModel model, string userId)
    {
        var seo = new Core.Models.Content.SeoData
        {
            Title = model.Seo.Title,
            Description = model.Seo.Description,
            Keywords = model.Seo.Keywords
        };

        return Commands.ImportStreamingAsync(
            userId,
            model.TwitchChannel,
            model.YouTubeChannel,
            model.Title,
            model.Slug,
            model.ScheduleDate,
            model.StartingTime,
            model.EndingTime,
            model.TwitchUrl,
            model.StreamingAbstract,
            model.YouTubeUrl,
            seo);
    }

    public Task<Guid> ScheduleStreamingAsync(ScheduleStreamingModel model, string userId)
    {
        var seo = new Core.Models.Content.SeoData
        {
            Title = model.Seo.Title,
            Description = model.Seo.Description,
            Keywords = model.Seo.Keywords
        };

        return Commands.ScheduleStreamingAsync(
            userId,
            model.TwitchChannel,
            model.YouTubeChannel,
            model.Title,
            model.Slug,
            model.ScheduleDate,
            model.StartingTime,
            model.EndingTime,
            model.TwitchUrl,
            model.YouTubeUrl,
            model.StreamingAbstract,
            seo);
    }

    public Task UpdateStreamingAsync(Guid streamingId, StreamingDetailModel model)
    {
        var seo = new Core.Models.Content.SeoData
        {
            Title = model.Seo.Title,
            Description = model.Seo.Description,
            Keywords = model.Seo.Keywords
        };

        return Commands.UpdateStreamingAsync(
            streamingId,
            model.TwitchChannel,
            model.YouTubeChannel,
            model.Title,
            model.ScheduleDate,
            model.StartingTime,
            model.EndingTime,
            model.TwitchUrl,
            model.StreamingAbstract,
            model.YouTubeUrl,
            seo);
    }

    public Task DeleteStreamingAsync(Guid streamingId) => Commands.DeleteStreamingAsync(streamingId);

    public async Task<StreamingStatsModel?> GetStreamingStatsAsync(string userId)
    {
        var streamingsQuery = Database.Streamings
            .ByUserId(userId)
            .OrderedBySchedule();

        var deliveredStreamingsNumber = await streamingsQuery.DeliveredOnly().CountAsync();
        var scheduledStreamingsNumber = await streamingsQuery.Scheduled().CountAsync();

        return new StreamingStatsModel(deliveredStreamingsNumber, scheduledStreamingsNumber);
    }
}
