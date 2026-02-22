using NavegaStudio.Areas.Backtesting.Models;
using NavegaStudio.Areas.Backtesting.Services;
using Microsoft.AspNetCore.Mvc;

namespace NavegaStudio.Areas.Backtesting.Controllers;

[Area("Backtesting")]
public class BacktestController : Controller
{
    private readonly BacktestService _backtestService;

    public BacktestController(BacktestService backtestService)
    {
        _backtestService = backtestService;
    }

    public IActionResult Index()
    {
        return View(new BacktestRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Run(BacktestRequest request)
    {
        if (!ModelState.IsValid)
            return View("Index", request);

        try
        {
            var result = await _backtestService.RunAsync(request);
            return View("Results", result);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Index", request);
        }
    }
}
