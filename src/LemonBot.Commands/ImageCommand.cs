using LemonBot.Commands.Attributes;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace LemonBot.Commands
{
    [BotCommand("!image", HelpText = "Show an image in overlay")]
    public class ImageCommand : IBotCommand
    {
        public async Task ExecuteAsync(BotCommandContext context)
        {
            if (context.Connection.State == HubConnectionState.Disconnected)
            {
                await context.Connection.StartAsync();
            }

            await context.Connection.InvokeAsync(
                "SendOverlay",
                "https://cdn.pixabay.com/photo/2015/04/27/22/53/man-742766_960_720.jpg");
        }
    }
}
