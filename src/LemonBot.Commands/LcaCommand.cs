namespace LemonBot.Commands;

[BotCommand("!lca", Comparison = CommandComparison.Equal, HelpText = "Mostra un link per unirsi al canale Discord della Lemon Code Academy")]
public class LcaCommand : IBotCommand
{
    private readonly TwitchClientProxy _client;
    private readonly ILogger<LcaCommand> _logger;

    private readonly string _discordUrl = "https://discord.gg/QYdgqMeSHu";

    public LcaCommand(TwitchClientProxy client, ILogger<LcaCommand> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task ExecuteAsync(BotCommandContext context)
    {
        _client.SendMessage($"Unisciti anche tu al canale Discord della Lemon Code Academy -> {_discordUrl}");
        return Task.CompletedTask;
    }
}
