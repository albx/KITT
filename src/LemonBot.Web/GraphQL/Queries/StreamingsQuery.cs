using KITT.Core.ReadModels;
using LemonBot.Web.GraphQL.Models;

namespace LemonBot.Web.GraphQL.Queries;

public class StreamingsQuery
{
    public IQueryable<Streaming> GetStreamings([Service(ServiceKind.Synchronized)] IDatabase database)
    {
        return database.Streamings
            .OrderedBySchedule()
            .Select(s => new Streaming
            {
                Id = s.Id,
                Abstract = s.Abstract,
                EndingTime = s.EndingTime,
                HostingChannelUrl = s.HostingChannelUrl,
                ScheduleDate = s.ScheduleDate,
                Seo = s.Seo != null ? new() { Title = s.Seo.Title, Description = s.Seo.Description, Keywords = s.Seo.Keywords } : new(),
                Slug = s.Slug,
                StartingTime = s.StartingTime,
                Title = s.Title,
                YouTubeVideoUrl = s.YouTubeVideoUrl
            });
    }
}
