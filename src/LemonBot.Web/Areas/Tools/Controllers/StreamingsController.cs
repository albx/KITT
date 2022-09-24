using KITT.Web.Models.Tools;
using LemonBot.Web.Areas.Tools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LemonBot.Web.Areas.Tools.Controllers;

[Route("api/tools/[controller]")]
[ApiController]
[Authorize]
public class StreamingsController : ControllerBase
{
    public StreamingsControllerServices ControllerServices { get; }

    public StreamingsController(StreamingsControllerServices controllerServices)
    {
        ControllerServices = controllerServices ?? throw new ArgumentNullException(nameof(controllerServices));
    }

    [HttpGet("scheduled")]
    public IActionResult GetScheduledStreamings()
    {
        var model = ControllerServices.GetScheduledStreamings(User.GetUserId());
        return Ok(model);
    }

    [HttpPost("{id}/stats")]
    public async Task<IActionResult> SaveStreamingStats(Guid id, [FromBody] StreamingStats model)
    {
        await ControllerServices.SaveStreamingStatsAsync(id, model);
        return Ok();
    }
}
