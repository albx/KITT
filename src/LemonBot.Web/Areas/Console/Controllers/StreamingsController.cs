using KITT.Web.Models.Streamings;
using LemonBot.Web.Areas.Console.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LemonBot.Web.Areas.Console.Controllers;

[Route("api/console/[controller]")]
[ApiController]
[Authorize]
public class StreamingsController : ControllerBase
{
    public StreamingsControllerServices ControllerServices { get; }

    public StreamingsController(StreamingsControllerServices controllerServices)
    {
        ControllerServices = controllerServices ?? throw new ArgumentNullException(nameof(controllerServices));
    }

    [HttpGet]
    public IActionResult GetAllStreamings()
    {
        var userId = User.GetUserId();

        var model = ControllerServices.GetAllStreamings(userId);
        return Ok(model);
    }

    [HttpGet("{id}")]
    public IActionResult GetStreamingDetail(Guid id)
    {
        var model = ControllerServices.GetStreamingDetail(id);
        if (model is null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> ScheduleStreaming([FromBody] ScheduleStreamingModel model)
    {
        var scheduledStreamingId = await ControllerServices.ScheduleStreamingAsync(model, User.GetUserId());
        return CreatedAtAction(nameof(GetStreamingDetail), new { id = scheduledStreamingId }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStreaming(Guid id, [FromBody]StreamingDetailModel model)
    {
        await ControllerServices.UpdateStreamingAsync(id, model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStreaming(Guid id)
    {
        await ControllerServices.DeleteStreamingAsync(id);
        return Ok();
    }
}
