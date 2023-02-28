//namespace LemonBot.Commands;

//[BotCommand("!image", HelpText = "Show an image in overlay")]
//public class ImageCommand : IBotCommand
//{
//    private readonly BotClient _botClient;

//    public ImageCommand(BotClient botClient)
//    {
//        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
//    }

//    public async Task ExecuteAsync(BotCommandContext context)
//    {
//        await _botClient.SendImageOverlayAsync("https://cdn.pixabay.com/photo/2015/04/27/22/53/man-742766_960_720.jpg");
//    }
//}
