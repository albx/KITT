namespace KITT.Core.Models;

public class StreamingStats
{
    public Guid Id { get; protected set; }

    public virtual Streaming Streaming { get; protected set; }

    public int Viewers { get; protected set; }

    public int Subscribers { get; protected set; }

    public int UserJoinedNumber { get; protected set; }

    public int UserLeftNumber { get; protected set; }

    public static StreamingStats RegisterStats(Streaming streaming, int viewers, int subscribers, int userJoinedNumber, int userLeftNumber)
    {
        ArgumentNullException.ThrowIfNull(streaming);

        return new StreamingStats
        {
            Id = Guid.NewGuid(),
            Streaming = streaming,
            Viewers = viewers,
            Subscribers = subscribers,
            UserJoinedNumber = userJoinedNumber,
            UserLeftNumber = userLeftNumber
        };
    }
}
