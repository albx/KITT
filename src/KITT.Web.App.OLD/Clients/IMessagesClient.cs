using KITT.Web.Models.Messages;

namespace KITT.Web.App.Clients;

public interface IMessagesClient
{
    Task SendMessageAsync(SendMessageModel message);
}
