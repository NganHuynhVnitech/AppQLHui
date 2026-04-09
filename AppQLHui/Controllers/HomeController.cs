using AppQLHui.Models;
using AppQLHui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AppQLHui.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ReportService _reportService;

    public HomeController(ILogger<HomeController> logger, ReportService reportService)
    {
        _logger = logger;
        _reportService = reportService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _reportService.GetDashboardStatsAsync();
        return View(stats);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
