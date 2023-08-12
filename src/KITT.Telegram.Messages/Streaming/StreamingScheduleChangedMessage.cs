namespace KITT.Telegram.Messages.Streaming;

[QueueName("changed-streaming-schedules")]
public record StreamingScheduleChangedMessage(
    Guid StreamingId,
    string StreamingTitle,
    string StreamingSlug,
    DateOnly StreamingScheduleDate,
    TimeOnly StreamingStartingTime,
    TimeOnly StreamingEndingTime);
