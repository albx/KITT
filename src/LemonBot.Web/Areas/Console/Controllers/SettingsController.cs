using KITT.Web.Models.Settings;
using LemonBot.Web.Areas.Console.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LemonBot.Web.Areas.Console.Controllers;

[Route("api/console/[controller]")]
[ApiController]
[Authorize]
public class SettingsController : ControllerBase
{
    public SettingsControllerServices ControllerServices { get; }

    public SettingsController(SettingsControllerServices controllerServices)
    {
        ControllerServices = controllerServices ?? throw new ArgumentNullException(nameof(controllerServices));
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
        var model = ControllerServices.GetAllSettings(User.GetUserId());
        return Ok(model);
    }

    [HttpGet("{id}")]
    public IActionResult GetSettingsDetail(Guid id)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewSettings([FromBody] CreateNewSettingsModel model)
    {
        var settingsId = await ControllerServices.CreateNewSettingsAsync(model, User.GetUserId());
        return CreatedAtAction(nameof(GetSettingsDetail), new { id = settingsId }, model);
    }
}
