namespace LemonBot.Commands;

public interface IBotCommand
{
    Task ExecuteAsync(BotCommandContext context);
}
