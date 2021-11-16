namespace LemonBot.Commands.Attributes;

public class BotCommandAttribute : Attribute
{
    public BotCommandAttribute(string prefix)
    {
        Prefix = prefix;
        Comparison = CommandComparison.Contains;
    }

    public string Prefix { get; }

    public string HelpText { get; set; } = string.Empty;

    public CommandComparison Comparison { get; set; }
}
