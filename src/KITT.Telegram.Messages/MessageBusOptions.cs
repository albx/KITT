namespace KITT.Telegram.Messages;

public record MessageBusOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
