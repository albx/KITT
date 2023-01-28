namespace LemonBot.Clients.Configurations;

public record HubOptions
{
    public string Endpoint { get; set; } = string.Empty;

    public string FunctionKey { get; set; } = string.Empty;
}
