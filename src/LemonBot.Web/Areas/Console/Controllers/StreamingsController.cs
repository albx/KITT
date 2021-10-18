using KITT.Web.Models.Streamings;
using LemonBot.Web.Areas.Console.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LemonBot.Web.Areas.Console.Controllers
{
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
            var model = ControllerServices.GetAllStreamings();
            return Ok(model);
        }

        [HttpGet("{id}")]
        public IActionResult GetStreamingDetail(Guid id)
        {
            var model = ControllerServices.GetStreamingDetail(id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleStreaming([FromBody]ScheduleStreamingModel model)
        {
            var scheduledStreamingId = await ControllerServices.ScheduleStreamingAsync(model);
            return CreatedAtAction(nameof(GetStreamingDetail), new { id = scheduledStreamingId }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStreaming(Guid id)
        {
            await ControllerServices.UpdateStreamingAsync(id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStreaming(Guid id)
        {
            await ControllerServices.DeleteStreamingAsync(id);
            return Ok();
        }
    }
}
