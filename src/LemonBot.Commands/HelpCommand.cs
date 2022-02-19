using LemonBot.Commands.Services;
using System.Text;

namespace LemonBot.Commands;

[BotCommand("!help", Comparison = CommandComparison.Equal, HelpText = "Mostra l'elenco dei comandi disponibili")]
public class HelpCommand : IBotCommand
{
    private readonly TwitchClientProxy _client;
    private readonly ILogger<HelpCommand> _logger;
    private readonly CommandsProvider _provider;

    public HelpCommand(TwitchClientProxy client, ILogger<HelpCommand> logger, CommandsProvider provider)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public Task ExecuteAsync(BotCommandContext context)
    {
        string helpText = CreateHelpText();
        _client.SendMessage(helpText);

        return Task.CompletedTask;
    }

    private string CreateHelpText()
    {
        var helpStringBuilder = new StringBuilder();
        foreach (var command in _provider.Commands)
        {
            helpStringBuilder.AppendLine($"{command.Prefix}: {command.HelpText}, ");
        }

        return helpStringBuilder.ToString();
    }
}
