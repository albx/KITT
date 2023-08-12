namespace KITT.Telegram.Messages;

public sealed class QueueNameAttribute : Attribute
{
    public string Name { get; }

    public QueueNameAttribute(string name)
    {
        Name = name;
    }
}
