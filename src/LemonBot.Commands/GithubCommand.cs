using LemonBot.Clients;
using LemonBot.Commands.Attributes;
using LemonBot.Commands.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace LemonBot.Commands
{
    [BotCommand("!github", Comparison = CommandComparison.StartsWith, HelpText = "Show the full repository url, based on the specified name")]
    public class GithubCommand : IBotCommand
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
            var repositoryName = message.Replace("!github", string.Empty).Trim();
            var fullRepositoryUrl = $"{_options.BaseUrl}/{repositoryName}";

            _client.SendMessage(fullRepositoryUrl);
            return Task.CompletedTask;
        }
    }
}
