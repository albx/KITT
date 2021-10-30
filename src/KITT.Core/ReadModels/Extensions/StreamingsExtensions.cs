using System.Linq;
using KITT.Core.Models;

namespace KITT.Core.ReadModels
{
    public static class StreamingsExtensions
    {
        public static IQueryable<Streaming> ByUserId(this IQueryable<Streaming> streamings, string userId) 
            => streamings.Where(s => s.UserId == userId);

        public static IQueryable<Streaming> OrderedBySchedule(this IQueryable<Streaming> streamings)
        {
            return streamings
                .OrderByDescending(s => s.ScheduleDate)
                .ThenBy(s => s.StartingTime)
                .ThenBy(s => s.EndingTime);
        }
    }
}
