namespace KITT.Web.App.Clients;

public interface IBotClient
{
    Task StartAsync();

    Task StopAsync();
}
