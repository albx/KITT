using LemonBot.Clients;
using LemonBot.Commands;
using LemonBot.Commands.Services;
using LemonBot.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace LemonBot.Services;

public class TwitchBotService : BackgroundService
{
    private readonly ILogger<TwitchBotService> _logger;

    private readonly TwitchClientProxy _client;

    private readonly BotCommandResolver _commandFactory;

    private readonly HubOptions _hubOptions;

    private HashSet<string> _usersAlreadyJoined;

    private HubConnection _connection;

    public TwitchBotService(TwitchClientProxy client, BotCommandResolver commandFactory, ILogger<TwitchBotService> logger, IOptions<HubOptions> hubOptions)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hubOptions = hubOptions?.Value ?? throw new ArgumentNullException(nameof(hubOptions));

        _connection = new HubConnectionBuilder()
            .WithUrl(_hubOptions.Endpoint)
            .Build();

        _usersAlreadyJoined = new();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _connection.StartAsync();
            await _connection.SendAsync("SendBotStart");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Connection failed: {Message}", ex.Message);
        }

        InitializeTwitchClient();
        ConnectToTwitch();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        UnregisterClientEvents();

        await _connection.StopAsync();
        await base.StopAsync(cancellationToken);
    }

    private void InitializeTwitchClient()
    {
        _client.Initialize();
        RegisterClientEvents();
    }

    private void UnregisterClientEvents()
    {
        _client.RemoveClientEvents(c =>
        {
            c.OnLog -= OnClientLog;
            c.OnConnected -= OnClientConnected;
            c.OnConnectionError -= OnConnectionErrorOccured;
            c.OnChatCommandReceived -= OnChatCommandReceived;
            c.OnMessageReceived -= OnMessageReceived;
            c.OnUserJoined -= OnUserJoined;
            c.OnUserLeft -= OnUserLeft;
        });
    }

    private void RegisterClientEvents()
    {
        _client.ConfigureClientEvents(c =>
        {
            c.OnLog += OnClientLog;
            c.OnConnected += OnClientConnected;
            c.OnConnectionError += OnConnectionErrorOccured;
            c.OnChatCommandReceived += OnChatCommandReceived;
            c.OnMessageReceived += OnMessageReceived;
            c.OnUserJoined += OnUserJoined;
            c.OnUserLeft += OnUserLeft;
        });
    }

    private async void OnUserLeft(object? sender, OnUserLeftArgs e)
    {
        _logger.LogInformation("User left the live stream");

        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }

        await _connection.InvokeAsync("SendUserLeft", e.Username);
    }

    private async void OnUserJoined(object? sender, OnUserJoinedArgs e)
    {
        _logger.LogInformation("User join the live stream");

        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }

        await _connection.InvokeAsync("SendUserJoin", e.Username);
    }

    private async Task ExecuteCommandByMessage(ChatMessage chatMessage)
    {
        var context = new BotCommandContext
        {
            UserName = chatMessage.Username,
            Message = chatMessage.Message,
            Connection = _connection
        };

        var command = _commandFactory.ResolveByMessage(chatMessage.Message);
        if (command is not null)
        {
            await command.ExecuteAsync(context);
        }
    }

    private void OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        _logger.LogInformation("{UserName} says: {Message}", e.ChatMessage.Username, e.ChatMessage.Message);

        //await ExecuteCommandByMessage(e.ChatMessage);
    }

    private async void OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        await ExecuteCommandByMessage(e.Command.ChatMessage);
    }

    private void OnConnectionErrorOccured(object? sender, OnConnectionErrorArgs e)
    {
        _logger.LogWarning("Error connecting {BotUsername}: {Error}", e.BotUsername, e.Error);
    }

    private void OnClientConnected(object? sender, OnConnectedArgs e)
    {
        _logger.LogInformation("{BotUsername} connected successfully", e.BotUsername);

        _client.Join();
        _client.SendMessage($"Ciao a tutti! Sono {e.BotUsername}, benvenuti nel canale :) ");
    }

    private void OnClientLog(object? sender, OnLogArgs e)
    {
        _logger.LogInformation("[{LogDate}] Bot: {BotUsername}, Data: {Data}", e.DateTime, e.BotUsername, e.Data);
    }

    private void ConnectToTwitch() => _client.Connect();
}
