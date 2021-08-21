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
    public class LivesController : ControllerBase
    {
        public LivesControllerServices ControllerServices { get; }

        public LivesController(LivesControllerServices controllerServices)
        {
            ControllerServices = controllerServices ?? throw new ArgumentNullException(nameof(controllerServices));
        }

        [HttpGet]
        public IActionResult GetAllLives()
        {
            var model = ControllerServices.GetAllLives();
            return Ok(model);
        }

        [HttpGet("{id}")]
        public IActionResult GetLiveDetail(Guid id)
        {
            var model = ControllerServices.GetLiveDetail(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewLive([FromBody]CreateLiveModel model)
        {
            var liveId = await ControllerServices.CreateNewLiveAsync(model);
            return CreatedAtAction(nameof(GetLiveDetail), new { id = liveId }, model);
        }
    }
}
