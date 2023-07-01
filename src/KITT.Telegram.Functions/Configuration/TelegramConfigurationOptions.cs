namespace KITT.Telegram.Functions.Configuration;

public record TelegramConfigurationOptions
{
    public string ChatId { get; set; } = string.Empty;
}
