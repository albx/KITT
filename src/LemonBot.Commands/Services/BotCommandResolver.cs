using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LemonBot.Commands.Services
{
    public class BotCommandResolver
    {
        private readonly IServiceProvider _provider;

        private readonly CommandsProvider _commandsProvider;

        public IDictionary<string, IBotCommand> CommandsMap { get; }

        public BotCommandResolver(IServiceProvider provider, CommandsProvider commandsProvider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _commandsProvider = commandsProvider ?? throw new ArgumentNullException(nameof(commandsProvider));
        }

        public IBotCommand ResolveByMessage(string message)
        {
            foreach (var commandDescriptor in _commandsProvider.Commands)
            {
                if (message.Contains(commandDescriptor.Prefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    var command = _provider.GetRequiredService(commandDescriptor.CommandType) as IBotCommand;
                    return command;
                }
            }

            return null;
        }
    }
}
