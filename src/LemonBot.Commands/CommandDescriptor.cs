namespace LemonBot.Commands;

public record CommandDescriptor(string Prefix, string HelpText, CommandComparison Comparison, Type CommandType)
{
    public bool CommandCanBeActivated(string message)
    {
        return Comparison switch
        {
            CommandComparison.Equal => message.Equals(Prefix, StringComparison.InvariantCultureIgnoreCase),
            CommandComparison.StartsWith => message.StartsWith(Prefix!, StringComparison.InvariantCultureIgnoreCase),
            CommandComparison.Contains => message.Contains(Prefix!, StringComparison.InvariantCultureIgnoreCase),
            _ => throw new ArgumentOutOfRangeException(nameof(Comparison))
        };
    }
}