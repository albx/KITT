namespace LemonBot.Web.Configuration;

public record BotConfiguration
{
    public string Endpoint { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
