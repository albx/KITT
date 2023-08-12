namespace KITT.Telegram.Messages.Streaming;

[QueueName("changed-streaming-hosting-channels")]
public record StreamingHostingChannelChangedMessage(
    Guid StreamingId,
    string StreamingTitle,
    string StreamingSlug,
    string StreamingHostingChannelUrl,
    DateOnly StreamingScheduleDate,
    TimeOnly StreamingStartingTime);
