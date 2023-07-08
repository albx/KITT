namespace KITT.Telegram.Messages.Streaming;

[QueueName("uploaded-streaming-videos")]
public record StreamingVideoUploadedMessage(
    Guid StreamingId,
    string StreamingTitle,
    string StreamingSlug,
    string YouTubeUrl);
