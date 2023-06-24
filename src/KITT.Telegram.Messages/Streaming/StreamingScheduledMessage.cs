namespace KITT.Telegram.Messages.Streaming;

[QueueName("scheduled-streamings")]
public record StreamingScheduledMessage(
    Guid StreamingId,
    string StreamingTitle,
    string StreamingSlug,
    DateOnly StreamingScheduledDate,
    TimeOnly StreamingStartingTime,
    TimeOnly StreamingEndingTime,
    string StreamingHostingChannelUrl,
    string StreamingAbstract);
