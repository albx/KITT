using KITT.Telegram.Messages;
using KITT.Web.Models.Messages;

namespace LemonBot.Web.Areas.Console.Services;

public class MessagesControllerServices
{
    public IMessageBus MessageBus { get; }

    public MessagesControllerServices(IMessageBus messageBus)
    {
        MessageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    public async Task SendMessageAsync(SendMessageModel model)
    {
        var message = new SendTextMessage(model.Text);
        await MessageBus.SendAsync(message);
    }
}
