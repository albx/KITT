//namespace LemonBot.Commands;

//[BotCommand("!ask", Comparison = CommandComparison.StartsWith, HelpText = "Ask a question")]
//public class AskCommand : TextResponse, IBotCommand
//{
//    private readonly BotClient _botClient;

//    public AskCommand(BotClient botClient)
//    {
//        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
//    }

//    public async Task ExecuteAsync(BotCommandContext context)
//    {
//        var message = RemovePrefixFromMessage(context.Message);
//        await _botClient.SendTextOverlayAsync(context.UserName, message);
//    }
//}
