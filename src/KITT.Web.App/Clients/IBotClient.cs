using KITT.Web.Models.Tools;

namespace KITT.Web.App.Clients;

public interface IBotClient
{
    Task StartAsync();

    Task StopAsync();

    Task<BotJobDetail?> GetDetailsAsync();
}
