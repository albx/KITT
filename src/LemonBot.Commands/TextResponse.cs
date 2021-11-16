using System.Reflection;

namespace LemonBot.Commands;

public abstract class TextResponse
{
    protected string RemovePrefixFromMessage(string message)
    {
        var messagePrefix = this.GetType().GetCustomAttribute<BotCommandAttribute>()?.Prefix;
        if (messagePrefix is null)
        {
            return message;
        }

        var finalText = message.Replace(messagePrefix, string.Empty).Trim();
        return finalText;
    }
}
