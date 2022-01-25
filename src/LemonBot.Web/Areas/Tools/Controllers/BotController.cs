using LemonBot.Web.Areas.Tools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LemonBot.Web.Areas.Tools.Controllers;

[Route("api/tools/[controller]")]
[ApiController]
[Authorize]
public class BotController : ControllerBase
{
    public BotHttpClient Client { get; }

    public BotController(BotHttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartBot()
    {
        await Client.StartAsync();
        return Ok();
    }

    [HttpPost("stop")]
    public async Task<IActionResult> StopBot()
    {
        await Client.StopAsync();
        return Ok();
    }
}
