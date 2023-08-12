using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace KITT.Telegram.Messages;

public class QueueStorageMessageBus : IMessageBus
{
    private readonly MessageBusOptions _options;
    private readonly ILogger<QueueStorageMessageBus> _logger;

    public QueueStorageMessageBus(IOptions<MessageBusOptions> options, ILogger<QueueStorageMessageBus> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendAsync<TMessage>(TMessage message)
        where TMessage : class
    {
        try
        {
            var queueName = message.GetType().GetCustomAttribute<QueueNameAttribute>()!.Name;

            var client = new QueueClient(
                _options.ConnectionString,
                queueName,
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            await client.CreateIfNotExistsAsync();

            var data = new BinaryData(message);
            await client.SendMessageAsync(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message {@Message}: {ErrorMessage}", message, ex.Message);
        }
    }
}
