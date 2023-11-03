using Microsoft.Extensions.DependencyInjection;

namespace LemonBot.Commands.Services;

public class BotCommandResolver
{
    private readonly IServiceProvider _provider;

    private readonly CommandsProvider _commandsProvider;

    private readonly ILogger<BotCommandResolver> _logger;

    public BotCommandResolver(IServiceProvider provider, CommandsProvider commandsProvider, ILogger<BotCommandResolver> logger)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _commandsProvider = commandsProvider ?? throw new ArgumentNullException(nameof(commandsProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IBotCommand ResolveByMessage(string message)
    {
        foreach (var commandDescriptor in _commandsProvider.Commands)
        {
            try
            {
                if (commandDescriptor.CommandCanBeActivated(message))
                {
                    var command = _provider.GetRequiredService(commandDescriptor.CommandType) as IBotCommand;

                    if (command == null)
                    {
                        throw new InvalidCastException($"Command {commandDescriptor.CommandType.Name} does not implement {nameof(IBotCommand)}");
                    }
                    return command;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving command {CommandPrefix}: {ErrorMessage}", commandDescriptor.Prefix, ex.Message);
            }
        }

        return null;
    }
}
