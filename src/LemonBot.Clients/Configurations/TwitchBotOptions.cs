namespace LemonBot.Clients.Configurations;

public record TwitchBotOptions
{
    public string BotUsername { get; init; } = string.Empty;

    public string AccessToken { get; init; } = string.Empty;

    public string ChannelName { get; init; } = string.Empty;
}
