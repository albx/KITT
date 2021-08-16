using LemonBot.Clients.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace LemonBot.Clients
{
    public class TwitchClientProxy
    {
        private readonly TwitchClient _client;

        private readonly TwitchBotOptions _options;

        private readonly ILogger<TwitchClientProxy> _logger;

        public TwitchClientProxy(TwitchClient client, IOptions<TwitchBotOptions> options, ILogger<TwitchClientProxy> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public void Initialize()
        {
            ConnectionCredentials connectionCredential = null;

            try
            {
                connectionCredential = new ConnectionCredentials(_options.BotUsername, _options.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to build connection credentials: {ErrorMessage}", ex.Message);
            }

            _client.Initialize(connectionCredential, _options.ChannelName);
        }

        public void ConfigureClientEvents(Action<TwitchClient> onConfiguringEvents)
        {
            if (onConfiguringEvents is null)
            {
                throw new ArgumentNullException(nameof(onConfiguringEvents));
            }

            onConfiguringEvents.Invoke(_client);
        }

        public void RemoveClientEvents(Action<TwitchClient> onRemovingEvents)
        {
            if (onRemovingEvents is null)
            {
                throw new ArgumentNullException(nameof(onRemovingEvents));
            }

            onRemovingEvents.Invoke(_client);
        }

        public void Connect() => _client.Connect();

        public void Join() => _client.JoinChannel(_options.ChannelName);

        public void SendMessage(string message) => _client.SendMessage(_options.ChannelName, message);
    }
}
