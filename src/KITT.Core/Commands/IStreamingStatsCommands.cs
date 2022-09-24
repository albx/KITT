namespace KITT.Core.Commands;

public interface IStreamingStatsCommands
{
    Task RegisterStreamingStatsAsync(
        Guid streamingId,
        int viewers,
        int subscribers,
        int userJoinedNumber,
        int userLeftNumber);
}
