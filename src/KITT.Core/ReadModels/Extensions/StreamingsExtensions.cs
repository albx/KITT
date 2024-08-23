namespace KITT.Core.ReadModels;

public static class StreamingsExtensions
{
    public static IQueryable<Streaming> ByUserId(this IQueryable<Streaming> streamings, string userId)
        => streamings.Where(s => s.UserId == userId);

    public static IQueryable<Streaming> OrderedBySchedule(this IQueryable<Streaming> streamings, bool ascending = true)
    {
        return ascending ?
            OrderedByScheduleAscending(streamings) : 
            OrderedByScheduleDescending(streamings);
    }

    public static IQueryable<Streaming> DeliveredOnly(this IQueryable<Streaming> streamings) 
        => streamings.Where(s => s.ScheduleDate < DateOnly.FromDateTime(DateTime.Today));

    public static IQueryable<Streaming> Scheduled(this IQueryable<Streaming> streamings)
    {
        return streamings.Where(s => s.ScheduleDate >= DateOnly.FromDateTime(DateTime.Today));
    }

    private static IQueryable<Streaming> OrderedByScheduleAscending(IQueryable<Streaming> streamings)
    {
        return streamings
            .OrderBy(s => s.ScheduleDate)
            .ThenBy(s => s.StartingTime)
            .ThenBy(s => s.EndingTime);
    }

    private static IQueryable<Streaming> OrderedByScheduleDescending(IQueryable<Streaming> streamings)
    {
        return streamings
            .OrderByDescending(s => s.ScheduleDate)
            .ThenByDescending(s => s.StartingTime)
            .ThenByDescending(s => s.EndingTime);
    }
}
