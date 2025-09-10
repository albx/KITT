using KITT.Telegram.Functions.Configuration;
using KITT.Telegram.Functions.Helpers;
using KITT.Telegram.Messages.Streaming;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace KITT.Telegram.Functions;

public class StreamingNotificationFunction
{
    private readonly ILogger _logger;

    private readonly TelegramBotClient _telegramClient;

    private readonly TelegramConfigurationOptions _telegramOptions;

    public StreamingNotificationFunction(
        ILoggerFactory loggerFactory,
        TelegramBotClient telegramClient,
        IOptions<TelegramConfigurationOptions> telegramOptions)
    {
        _logger = loggerFactory.CreateLogger<StreamingNotificationFunction>();
        _telegramClient = telegramClient ?? throw new ArgumentNullException(nameof(telegramClient));
        _telegramOptions = telegramOptions?.Value ?? throw new ArgumentNullException(nameof(telegramOptions));
    }

    #region scheduled streamings
    [Function(nameof(NotifyScheduledStreaming))]
    public async Task NotifyScheduledStreaming(
        [QueueTrigger("scheduled-streamings")] StreamingScheduledMessage message)
    {
        try
        {
            var messageText = MessageConverter.ToText(message);

            var messageSent = await _telegramClient.SendMessage(
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
    #endregion

    #region Canceled streamings
    [Function(nameof(NotifyCanceledStreaming))]
    public async Task NotifyCanceledStreaming(
        [QueueTrigger("canceled-streamings")] StreamingCanceledMessage message)
    {
        try
        {
            var messageText = MessageConverter.ToText(message);

            var messageSent = await _telegramClient.SendMessage(
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
    #endregion

    #region Changed streaming schedule
    [Function(nameof(NotifyChangeSchedule))]
    public async Task NotifyChangeSchedule(
        [QueueTrigger("changed-streaming-schedules")] StreamingScheduleChangedMessage message)
    {
        try
        {
            var messageText = MessageConverter.ToText(message);

            await _telegramClient.SendMessage(
                _telegramOptions.ChatId,
                messageText,
                parseMode: ParseMode.MarkdownV2);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending streaming schedule change message: {ErrorMessage}",
                ex.Message);

            throw;
        }
    }
    #endregion

    #region Streaming video uploaded
    [Function(nameof(NotifyStreamingVideoUploaded))]
    public async Task NotifyStreamingVideoUploaded(
        [QueueTrigger("uploaded-streaming-videos")] StreamingVideoUploadedMessage message)
    {
        try
        {
            var messageText = MessageConverter.ToText(message);

            await _telegramClient.SendMessage(
                _telegramOptions.ChatId,
                messageText,
                parseMode: ParseMode.MarkdownV2);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending streaming video uploaded message: {ErrorMessage}",
                ex.Message);

            throw;
        }
    }
    #endregion

    #region Hosting channel changed
    [Function(nameof(NotifyStreamingHostingChannelChanged))]
    public async Task NotifyStreamingHostingChannelChanged(
        [QueueTrigger("changed-streaming-hosting-channels")] StreamingHostingChannelChangedMessage message)
    {
        try
        {
            var messageText = MessageConverter.ToText(message);

            await _telegramClient.SendMessage(
                _telegramOptions.ChatId,
                messageText,
                parseMode: ParseMode.MarkdownV2);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending streaming hosting channel changed message: {ErrorMessage}",
                ex.Message);

            throw;
        }
    }
    #endregion
}
