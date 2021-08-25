using KITT.Web.Models.Lives;
using LemonBot.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LemonBot.Web.Controllers.KITT
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleStreaming([FromBody]ScheduleStreamingModel model)
        {
            var liveId = await ControllerServices.ScheduleStreamingAsync(model);
            return CreatedAtAction(nameof(GetStreamingDetail), new { id = liveId }, model);
        }
    }
}
