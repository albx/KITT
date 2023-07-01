using KITT.Telegram.Functions.Configuration;
using KITT.Telegram.Messages.Streaming;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace KITT.Telegram.Functions
{
    public class TelegramNotificationFunction
    {
        private readonly ILogger _logger;

        private readonly TelegramBotClient _telegramClient;

        private readonly TelegramConfigurationOptions _telegramOptions;

        public TelegramNotificationFunction(
            ILoggerFactory loggerFactory,
            TelegramBotClient telegramClient,
            IOptions<TelegramConfigurationOptions> telegramOptions)
        {
            _logger = loggerFactory.CreateLogger<TelegramNotificationFunction>();
            _telegramClient = telegramClient ?? throw new ArgumentNullException(nameof(telegramClient));
            _telegramOptions = telegramOptions?.Value ?? throw new ArgumentNullException(nameof(telegramOptions));
        }

        [Function(nameof(NotifyScheduledStreaming))]
        public async Task NotifyScheduledStreaming(
            [QueueTrigger("scheduled-streamings")] StreamingScheduledMessage message)
        {
            try
            {
                var messageText = ConvertMessageToText(message);

                var messageSent = await _telegramClient.SendTextMessageAsync(
                    _telegramOptions.ChatId,
                    messageText,
                    parseMode: ParseMode.MarkdownV2);

                _logger.LogInformation(
                    "Message sent correctly for scheduled streaming {StreamingTitle}",
                    message.StreamingTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending scheduled streaming message: {ErrorMessage}",
                    ex.Message);

                throw;
            }

        }

        private string ConvertMessageToText(StreamingScheduledMessage message)
        {
            var messageText = $"""
                *Nuova live "{message.StreamingTitle}" pianificata!*
                Il {message.StreamingScheduledDate.ToShortDateString()} si va live dalle {message.StreamingStartingTime.ToShortTimeString()} alle {message.StreamingEndingTime.ToShortTimeString()}.
                Vi aspetto su [{message.StreamingHostingChannelUrl}]({message.StreamingHostingChannelUrl}) per scoprire insieme di cosa tratteremo!
                Trovate maggiori informazioni su [https://live.morialberto.it/d/{message.StreamingSlug}](https://live.morialberto.it/d/{message.StreamingSlug})
                """;

            return Escape(messageText);
        }

        [Function(nameof(NotifyCanceledStreaming))]
        public async Task NotifyCanceledStreaming(
            [QueueTrigger("canceled-streamings")] StreamingCanceledMessage message)
        {
            try
            {
                var messageText = ConvertMessageToText(message);

                var messageSent = await _telegramClient.SendTextMessageAsync(
                    _telegramOptions.ChatId,
                    messageText,
                    parseMode: ParseMode.MarkdownV2);

                _logger.LogInformation(
                    "Message sent correctly for scheduled streaming {StreamingTitle}",
                    message.StreamingTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending canceled streaming message: {ErrorMessage}",
                    ex.Message);

                throw;
            }
        }

        private string ConvertMessageToText(StreamingCanceledMessage message)
        {
            var messageText = $"""
                La live "{message.StreamingTitle}" prevista per il giorno {message.StreamingScheduledDate.ToShortDateString()} dalle {message.StreamingStartingTime.ToShortTimeString()} alle {message.StreamingEndingTime.ToShortTimeString()} è stata annullata!
                Mi scuso per il problema. La recupereremo il prima possibile!
                """;

            return Escape(messageText);
        }

        private string Escape(string text)
        {
            string[] chars = new[]
            {
                 ">", "#", "+", "-", "=", "|", "{", "}", ".", "!"
            };

            foreach (var item in chars)
            {
                text = text.Replace(item, $"\\{item}");
            }

            return text;
        }
    }
}
