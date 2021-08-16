using LemonBot.Clients;
using LemonBot.Commands.Attributes;
using LemonBot.Commands.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LemonBot.Commands
{
    [BotCommand("!help", HelpText = "Show some help for BOT commands")]
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
                helpStringBuilder.AppendLine($"{command.Prefix}: {command.HelpText}");
            }

            return helpStringBuilder.ToString();
        }
    }
}