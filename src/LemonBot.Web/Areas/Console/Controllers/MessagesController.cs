using KITT.Web.Models.Messages;
using LemonBot.Web.Areas.Console.Services;
using Microsoft.AspNetCore.Mvc;

namespace LemonBot.Web.Areas.Console.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    public MessagesControllerServices ControllerServices { get; }

    public MessagesController(MessagesControllerServices controllerServices)
    {
        ControllerServices = controllerServices ?? throw new ArgumentNullException(nameof(controllerServices));
    }

    [HttpPost("")]
    public async Task<IActionResult> SendMessages([FromBody] SendMessageModel model)
    {
        await ControllerServices.SendMessageAsync(model);
        return Accepted();
    }
}
