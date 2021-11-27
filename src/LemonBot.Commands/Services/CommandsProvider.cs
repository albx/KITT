using LemonBot.Commands.Attributes;
using System.Reflection;

namespace LemonBot.Commands.Services;

public class CommandsProvider
{
    public IEnumerable<CommandDescriptor> Commands { get; }

    public CommandsProvider()
    {
        Commands = ResolveAvailableCommands();
    }

    private IEnumerable<CommandDescriptor> ResolveAvailableCommands()
    {
        var botInterfaceType = typeof(IBotCommand);
        var commandTypes = botInterfaceType.Assembly.GetTypes()
            .Where(t => botInterfaceType.IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<BotCommandAttribute>() != null)
            .Where(t => t.IsClass)
            .Select(t => new CommandDescriptor
            {
                CommandType = t,
                HelpText = t.GetCustomAttribute<BotCommandAttribute>()?.HelpText ?? string.Empty,
                Prefix = t.GetCustomAttribute<BotCommandAttribute>()?.Prefix ?? string.Empty,
                Comparison = t.GetCustomAttribute<BotCommandAttribute>()?.Comparison ?? CommandComparison.Contains
            }).ToArray();

        return commandTypes;
    }
}
