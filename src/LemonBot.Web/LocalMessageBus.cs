using KITT.Telegram.Messages;

namespace LemonBot.Web;

internal class LocalMessageBus : IMessageBus
{
    public Task SendAsync<TMessage>(TMessage message) where TMessage : class
    {
        return Task.CompletedTask;
    }
}
