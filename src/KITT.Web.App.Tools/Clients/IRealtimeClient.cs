namespace KITT.Web.App.Tools.Clients;

public interface IRealtimeClient
{
    void ConnectToEndpoint(Uri endpoint);

    IDisposable On<T>(string eventIdentifier, Action<T> handler);

    IDisposable On(string eventIdentifier, Action handler);

    Task StartAsync();
}
