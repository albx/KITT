using KITT.Telegram.Messages;

namespace KITT.Cms.Web.Api;

internal class LocalMessageBus : IMessageBus
{
    public Task SendAsync<TMessage>(TMessage message) where TMessage : class
    {
        return Task.CompletedTask;
    }
}
