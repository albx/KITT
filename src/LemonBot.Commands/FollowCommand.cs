using LemonBot.Clients;
using LemonBot.Commands.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LemonBot.Commands
{
    [BotCommand("!follow", HelpText = "Display the twitch url of the specified channel")]
    public class FollowCommand : IBotCommand
    {
        private readonly TwitchClientProxy _client;
        private readonly ILogger<FollowCommand> _logger;

        public FollowCommand(TwitchClientProxy client, ILogger<FollowCommand> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task ExecuteAsync(BotCommandContext context)
        {
            var message = context.Message;
            var twichChannelName = message.Replace("!follow", string.Empty).Trim();

            _client.SendMessage($"https://www.twitch.tv/{twichChannelName}");
            return Task.CompletedTask;
        }
    }
}
