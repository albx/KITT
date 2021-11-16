using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LemonBot.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Overlay()
    {
        return View();
    }
}
