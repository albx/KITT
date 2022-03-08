using LemonBot.Web.Areas.Tools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LemonBot.Web.Areas.Tools.Controllers;

[Route("api/tools/[controller]")]
[ApiController]
[Authorize]
public class BotController : ControllerBase
{
    public IBotHttpClient Client { get; }

    public BotController(IBotHttpClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
    }

    [HttpGet]
    public async Task<IActionResult> GetJobDetails()
    {
        try
        {
            var detail = await Client.GetDetailAsync();
            return Ok(detail);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
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
