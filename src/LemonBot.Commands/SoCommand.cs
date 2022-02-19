namespace LemonBot.Commands;

[BotCommand("!so", Comparison = CommandComparison.StartsWith, HelpText = "Mostra il canale twitch del nome indicato")]
public class SoCommand : TextResponse, IBotCommand
{
    private readonly TwitchClientProxy _client;
    private readonly ILogger<SoCommand> _logger;

    public SoCommand(TwitchClientProxy client, ILogger<SoCommand> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task ExecuteAsync(BotCommandContext context)
    {
        var message = context.Message;
        var twichChannelName = this.RemovePrefixFromMessage(message);

        _client.SendMessage($"Seguite https://www.twitch.tv/{twichChannelName} per mirabolanti contenuti");
        return Task.CompletedTask;
    }
}
