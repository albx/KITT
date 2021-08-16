namespace LemonBot.Clients.Configurations
{
    public record TwitchBotOptions
    {
        public string BotUsername { get; init; }

        public string AccessToken { get; init; }

        public string ChannelName { get; init; }
    }
}
