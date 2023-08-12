namespace KITT.Telegram.Messages;

[QueueName("text-messages")]
public record SendTextMessage(
    string Text);
