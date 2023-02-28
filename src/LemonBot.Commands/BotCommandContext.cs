namespace LemonBot.Commands;

public record BotCommandContext
{
    public string UserName { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    //public HubConnection Connection { get; set; }
}
