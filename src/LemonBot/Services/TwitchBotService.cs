using LemonBot.Clients;
using LemonBot.Commands;
using LemonBot.Commands.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace LemonBot.Services;

public class TwitchBotService : BackgroundService
{
    private readonly ILogger<TwitchBotService> _logger;

    private readonly TwitchClientProxy _twitchClient;

    private readonly BotCommandResolver _commandFactory;

    private readonly BotClient _botClient;

    private HashSet<string> _usersAlreadyJoined;

    public TwitchBotService(
        TwitchClientProxy twitchClient,
        BotCommandResolver commandFactory,
        BotClient botClient,
        ILogger<TwitchBotService> logger)
    {
        _twitchClient = twitchClient ?? throw new ArgumentNullException(nameof(twitchClient));
        _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        _botClient = botClient;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _usersAlreadyJoined = new();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeTwitchClient();
        ConnectToTwitch();

        await _botClient.NotifyStartAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        UnregisterClientEvents();
        await _botClient.NotifyStopAsync();

        await base.StopAsync(cancellationToken);
    }

    private void InitializeTwitchClient()
    {
        _twitchClient.Initialize();
        RegisterClientEvents();
    }

    private void UnregisterClientEvents()
    {
        _twitchClient.RemoveClientEvents(c =>
        {
            c.OnLog -= OnClientLog;
            c.OnConnected -= OnClientConnected;
            c.OnConnectionError -= OnConnectionErrorOccured;
            c.OnChatCommandReceived -= OnChatCommandReceived;
            c.OnMessageReceived -= OnMessageReceived;
            c.OnUserJoined -= OnUserJoined;
            c.OnUserLeft -= OnUserLeft;
            c.OnNewSubscriber -= OnNewUserSubscription;
        });
    }

    private void RegisterClientEvents()
    {
        _twitchClient.ConfigureClientEvents(c =>
        {
            c.OnLog += OnClientLog;
            c.OnConnected += OnClientConnected;
            c.OnConnectionError += OnConnectionErrorOccured;
            c.OnChatCommandReceived += OnChatCommandReceived;
            c.OnMessageReceived += OnMessageReceived;
            c.OnUserJoined += OnUserJoined;
            c.OnUserLeft += OnUserLeft;
            c.OnNewSubscriber += OnNewUserSubscription;
        });
    }

    private void OnNewUserSubscription(object? sender, OnNewSubscriberArgs e)
    {
        _logger.LogInformation("New user subscription");
        _twitchClient.SendMessage($"Ciao {e.Subscriber.DisplayName}, grazie per la tua subscription!");

        _botClient.SendNewUserSubscriptionAsync(e.Subscriber.DisplayName);
    }

    private void OnUserLeft(object? sender, OnUserLeftArgs e)
    {
        _logger.LogInformation("User left the live stream");
        _botClient.SendUserLeftAsync(e.Username);
    }

    private void OnUserJoined(object? sender, OnUserJoinedArgs e)
    {
        _logger.LogInformation("User join the live stream");
        _botClient.SendUserJoinAsync(e.Username);
    }

    private async Task ExecuteCommandByMessage(ChatMessage chatMessage)
    {
        var context = new BotCommandContext
        {
            UserName = chatMessage.Username,
            Message = chatMessage.Message
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

        _twitchClient.Join();
        _twitchClient.SendMessage($"Ciao a tutti! Sono {e.BotUsername}, benvenuti nel canale :) ");
    }

    private void OnClientLog(object? sender, OnLogArgs e)
    {
        _logger.LogInformation("[{LogDate}] Bot: {BotUsername}, Data: {Data}", e.DateTime, e.BotUsername, e.Data);
    }

    private void ConnectToTwitch() => _twitchClient.Connect();
}
