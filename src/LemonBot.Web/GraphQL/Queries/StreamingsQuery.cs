using KITT.Core.ReadModels;
using KITT.Core.ReadModels.Extensions;
using LemonBot.Web.GraphQL.Models;

namespace LemonBot.Web.GraphQL.Queries;

public class StreamingsQuery
{
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Streaming> GetStreamings([Service(ServiceKind.Synchronized)] IDatabase database)
    {
        return database.Streamings
            .PublishedOnly()
            .Select(s => new Streaming
            {
                Id = s.Id,
                Abstract = s.Abstract,
                EndingTime = s.EndingTime.ToTimeSpan(),
                HostingChannelUrl = s.HostingChannelUrl,
                ScheduleDate = s.ScheduleDate.ToDateTime(TimeOnly.MinValue),
                Seo = s.Seo != null ? new() { Title = s.Seo.Title, Description = s.Seo.Description, Keywords = s.Seo.Keywords } : new(),
                Slug = s.Slug,
                StartingTime = s.StartingTime.ToTimeSpan(),
                Title = s.Title,
                YouTubeVideoUrl = s.YouTubeVideoUrl
            });
    }

    public Streaming? GetStreamingBySlug([Service(ServiceKind.Synchronized)] IDatabase database, string slug)
    {
        var streaming = database.Streamings
            .PublishedOnly()
            .WithSlug(slug)
            .SingleOrDefault();

        if (streaming is null)
        {
            return null;
        }

        var response = new Streaming
        {
            Id = streaming.Id,
            Abstract = streaming.Abstract,
            EndingTime = streaming.EndingTime.ToTimeSpan(),
            HostingChannelUrl = streaming.HostingChannelUrl,
            ScheduleDate = streaming.ScheduleDate.ToDateTime(TimeOnly.MinValue),
            Seo = streaming.Seo != null ?
                new() { Title = streaming.Seo.Title, Description = streaming.Seo.Description, Keywords = streaming.Seo.Keywords }
                : new(),
            Slug = streaming.Slug,
            StartingTime = streaming.StartingTime.ToTimeSpan(),
            Title = streaming.Title,
            YouTubeVideoUrl = streaming.YouTubeVideoUrl
        };

        return response;
    }
}
