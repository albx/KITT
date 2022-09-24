namespace KITT.Core.Commands;

public class StreamingStatsCommands : IStreamingStatsCommands
{
    private readonly KittDbContext _context;

    public StreamingStatsCommands(KittDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task RegisterStreamingStatsAsync(Guid streamingId, int viewers, int subscribers, int userJoinedNumber, int userLeftNumber)
    {
        var streaming = _context.Streamings.SingleOrDefault(s => s.Id == streamingId);
        if (streaming is null)
        {
            throw new ArgumentOutOfRangeException(nameof(streamingId));
        }

        var stats = StreamingStats.RegisterStats(
            streaming,
            viewers,
            subscribers,
            userJoinedNumber,
            userLeftNumber);

        _context.Add(stats);
        return _context.SaveChangesAsync();
    }
}
