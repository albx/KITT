namespace KITT.Core.ReadModels;

public static class StreamingStatsExtensions
{
    public static IQueryable<StreamingStats> ByStreaming(this IQueryable<StreamingStats> stats, Guid streamingId)
        => stats.Include(s => s.Streaming).Where(s => s.Streaming.Id == streamingId);
}
