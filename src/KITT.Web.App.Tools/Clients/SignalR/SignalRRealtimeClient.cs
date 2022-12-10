using Microsoft.AspNetCore.SignalR.Client;

namespace KITT.Web.App.Tools.Clients.SignalR;

public class SignalRRealtimeClient : IRealtimeClient
{
    private HubConnection? _connection;

    public void ConnectToEndpoint(Uri endpoint)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(endpoint)
            .Build();
    }

    public IDisposable On<T>(string eventIdentifier, Action<T> handler)
    {
        if (_connection is null)
        {
            throw new InvalidOperationException("SignalR connection must be initialized");
        }

        return _connection.On<T>(eventIdentifier, handler);
    }

    public IDisposable On(string eventIdentifier, Action handler)
    {
        if (_connection is null)
        {
            throw new InvalidOperationException("SignalR connection must be initialized");
        }

        return _connection.On(eventIdentifier, handler);
    }

    public Task StartAsync()
    {
        if (_connection is null)
        {
            throw new InvalidOperationException("SignalR connection must be initialized");
        }

        return _connection.StartAsync();
    }
}
