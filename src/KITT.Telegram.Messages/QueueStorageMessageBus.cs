using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace KITT.Telegram.Messages;

public class QueueStorageMessageBus : IMessageBus
{
    private readonly MessageBusOptions _options;

    public QueueStorageMessageBus(IOptions<MessageBusOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task SendAsync<TMessage>(TMessage message)
        where TMessage : class
    {

        var queueName = typeof(TMessage).GetCustomAttribute<QueueNameAttribute>()!.Name;

        var client = new QueueClient(
            _options.ConnectionString,
            queueName,
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

        await client.CreateIfNotExistsAsync();

        var data = new BinaryData(message);
        await client.SendMessageAsync(data);
    }
}
