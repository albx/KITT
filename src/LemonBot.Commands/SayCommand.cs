using LemonBot.Clients;
using LemonBot.Commands.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LemonBot.Commands
{
    [BotCommand("!say", HelpText = "Say something to the chat")]
    public class SayCommand : IBotCommand
    {
        private readonly TwitchClientProxy _client;
        private readonly ILogger<SayCommand> _logger;

        public SayCommand(TwitchClientProxy client, ILogger<SayCommand> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task ExecuteAsync(BotCommandContext context)
        {
            var userName = context.UserName;
            var message = context.Message;

            var messageToSend = $"{userName} says: {message.Replace("!say", string.Empty)}";
            _client.SendMessage(messageToSend);

            return Task.CompletedTask;
        }
    }
}
