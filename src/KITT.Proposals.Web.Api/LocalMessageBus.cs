using KITT.Telegram.Messages;

namespace KITT.Proposals.Web.Api;

internal class LocalMessageBus : IMessageBus
{
    public Task SendAsync<TMessage>(TMessage message) where TMessage : class
    {
        return Task.CompletedTask;
    }
}
