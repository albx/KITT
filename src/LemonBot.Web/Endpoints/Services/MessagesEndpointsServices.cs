using KITT.Telegram.Messages;
using KITT.Web.Models.Messages;

namespace LemonBot.Web.Endpoints.Services;

public class MessagesEndpointsServices
{
    public IMessageBus MessageBus { get; }

    public MessagesEndpointsServices(IMessageBus messageBus)
    {
        MessageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    public async Task SendMessageAsync(SendMessageModel model)
    {
        var message = new SendTextMessage(model.Text);
        await MessageBus.SendAsync(message);
    }
}
