namespace LemonBot.Commands;

public record CommandDescriptor
{
    public string Prefix { get; init; }

    public string HelpText { get; init; }

    public CommandComparison Comparison { get; init; }

    public Type CommandType { get; init; }

    public bool CommandCanBeActivated(string message)
    {
        return Comparison switch
        {
            CommandComparison.Equal => message.Equals(Prefix, StringComparison.InvariantCultureIgnoreCase),
            CommandComparison.StartsWith => message.StartsWith(Prefix, StringComparison.InvariantCultureIgnoreCase),
            CommandComparison.Contains => message.Contains(Prefix, StringComparison.InvariantCultureIgnoreCase),
            _ => throw new ArgumentOutOfRangeException(nameof(Comparison))
        };
    }
}
