namespace KITT.Core.ReadModels;

public static class StreamingsExtensions
{
    extension(IQueryable<Streaming> streamings)
    {
        public IQueryable<Streaming> ByUserId(string userId) => streamings.Where(s => s.UserId == userId);

        public IQueryable<Streaming> OrderedBySchedule(bool ascending = true)
        {
            return ascending ?
                OrderedByScheduleAscending(streamings) :
                OrderedByScheduleDescending(streamings);
        }

        public IQueryable<Streaming> DeliveredOnly() => streamings.Where(s => s.ScheduleDate < DateOnly.FromDateTime(DateTime.Today));

        public IQueryable<Streaming> Scheduled()
        {
            return streamings.Where(s => s.ScheduleDate >= DateOnly.FromDateTime(DateTime.Today));
        }
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
