using KITT.Telegram.Functions.Configuration;
using KITT.Telegram.Functions.Helpers;
using KITT.Telegram.Messages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace KITT.Telegram.Functions;

public class TextNotificationFunction
{
    private readonly ILogger _logger;

    private readonly TelegramBotClient _telegramClient;

    private readonly TelegramConfigurationOptions _telegramOptions;

    public TextNotificationFunction(
        ILoggerFactory loggerFactory,
        TelegramBotClient telegramClient,
        IOptions<TelegramConfigurationOptions> telegramOptions)
    {
        _logger = loggerFactory.CreateLogger<TextNotificationFunction>();
        _telegramClient = telegramClient ?? throw new ArgumentNullException(nameof(telegramClient));
        _telegramOptions = telegramOptions?.Value ?? throw new ArgumentNullException(nameof(telegramOptions));
    }

    [Function(nameof(NotifyTextMessage))]
    public async Task NotifyTextMessage(
        [QueueTrigger("text-messages")] SendTextMessage message)
    {
        try
        {
            var messageText = MessageConverter.ToText(message);

            var messageSent = await _telegramClient.SendTextMessageAsync(
                _telegramOptions.ChatId,
                messageText,
                parseMode: ParseMode.MarkdownV2);

            _logger.LogInformation("Message sent correctly");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending text message: {ErrorMessage}",
                ex.Message);

            throw;
        }
    }
}
