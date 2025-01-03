using KITT.Telegram.Messages;

namespace KITT.Web.App;

internal class LocalMessageBus : IMessageBus
{
    public Task SendAsync<TMessage>(TMessage message) where TMessage : class
    {
        return Task.CompletedTask;
    }
}
