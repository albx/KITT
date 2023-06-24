using KITT.Telegram.Messages.Streaming;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;

namespace KITT.Telegram.Functions
{
    public class TelegramNotificationFunction
    {
        private readonly ILogger _logger;

        public TelegramNotificationFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TelegramNotificationFunction>();
        }

        [Function(nameof(NotifyScheduledStreaming))]
        public void NotifyScheduledStreaming(
            [QueueTrigger("scheduled-streamings")] StreamingScheduledMessage message)
        {
            //TODO
        }

        [Function(nameof(NotifyCanceledStreaming))]
        public void NotifyCanceledStreaming(
            [QueueTrigger("canceled-streamings")] StreamingCanceledMessage message)
        {
            //TODO
        }
    }
}
