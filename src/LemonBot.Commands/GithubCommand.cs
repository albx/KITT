using LemonBot.Commands.Options;
using Microsoft.Extensions.Options;

namespace LemonBot.Commands;

[BotCommand("!github", Comparison = CommandComparison.StartsWith, HelpText = "Mostra l'URL completo del repository GitHub basato sul nome")]
public class GithubCommand : TextResponse, IBotCommand
{
    private readonly TwitchClientProxy _client;
    private readonly ILogger<GithubCommand> _logger;
    private readonly GithubOptions _options;

    public GithubCommand(TwitchClientProxy client, IOptions<GithubOptions> options, ILogger<GithubCommand> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task ExecuteAsync(BotCommandContext context)
    {
        var message = context.Message;
        var repositoryName = this.RemovePrefixFromMessage(message);
        var fullRepositoryUrl = $"{_options.BaseUrl}/{repositoryName}";

        _client.SendMessage(fullRepositoryUrl);
        return Task.CompletedTask;
    }
}
