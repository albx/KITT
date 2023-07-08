namespace KITT.Telegram.Messages.Streaming;

[QueueName("canceled-streamings")]
public record class StreamingCanceledMessage(
    Guid StreamingId,
    string StreamingTitle,
    DateOnly StreamingScheduleDate,
    TimeOnly StreamingStartingTime,
    TimeOnly StreamingEndingTime);
