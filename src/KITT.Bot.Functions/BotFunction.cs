using KITT.Bot.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace KITT.Bot.Functions
{
    public static class BotFunction
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request,
            [SignalRConnectionInfo(HubName = "Bot")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        #region Bot Management calls
        [FunctionName(nameof(NotifyStart))]
        public static async Task NotifyStart(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            [SignalR(HubName = "Bot")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var notification = await request.ReadFromJsonAsync<StartNotification>();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "BotStarted",
                Arguments = new[] { notification }
            });
        }

        [FunctionName(nameof(NotifyStop))]
        public static async Task NotifyStop(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            [SignalR(HubName = "Bot")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var notification = await request.ReadFromJsonAsync<StopNotification>();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "BotStopped",
                Arguments = new[] { notification }
            });
        }

        [FunctionName(nameof(SendNewUserSubscription))]
        public static async Task SendNewUserSubscription(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            [SignalR(HubName = "Bot")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var subscription = await request.ReadFromJsonAsync<UserSubscription>();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "UserSubscriptionReceived",
                Arguments = new[] { subscription.SubscriberName }
            });
        }

        [FunctionName(nameof(SendUserLeft))]
        public static async Task SendUserLeft(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            [SignalR(HubName = "Bot")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var user = await request.ReadFromJsonAsync<UserLeft>();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "UserLeftReceived",
                Arguments = new[] { user.UserName }
            });
        }

        [FunctionName(nameof(SendUserJoin))]
        public static async Task SendUserJoin(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            [SignalR(HubName = "Bot")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var user = await request.ReadFromJsonAsync<UserJoin>();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "UserJoinReceived",
                Arguments = new[] { user.UserName }
            });
        }
        #endregion

        [FunctionName(nameof(SendImageOverlay))]
        public static async Task SendImageOverlay(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            [SignalR(HubName = "Bot")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var image = await request.ReadFromJsonAsync<ImageOverlay>();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "ImageOverlayReceived",
                Arguments = new[] { image.ResourceUrl }
            });
        }

        [FunctionName(nameof(SendTextOverlay))]
        public static async Task SendTextOverlay(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            [SignalR(HubName = "Bot")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var text = await request.ReadFromJsonAsync<TextOverlay>();

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "TextOverlayReceived",
                Arguments = new[] { text.UserName, text.Message }
            });
        }

        #region Overlay pages
        [FunctionName(nameof(Image))]
        public static IActionResult Image(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest request,
            ExecutionContext context)
        {
            var overlayFilePath = Path.Combine(context.FunctionAppDirectory, "content", "image.html");

            return new ContentResult
            {
                Content = File.ReadAllText(overlayFilePath),
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        [FunctionName(nameof(Text))]
        public static IActionResult Text(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest request,
            ExecutionContext context)
        {
            var overlayFilePath = Path.Combine(context.FunctionAppDirectory, "content", "text.html");

            return new ContentResult
            {
                Content = File.ReadAllText(overlayFilePath),
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
        #endregion
    }
}
