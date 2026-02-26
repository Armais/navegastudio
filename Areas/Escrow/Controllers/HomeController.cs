using Microsoft.AspNetCore.Mvc;
using NavegaStudio.Areas.Escrow.Models;
using NavegaStudio.Areas.Escrow.Services;

namespace NavegaStudio.Areas.Escrow.Controllers;

[Area("Escrow")]
public class HomeController : Controller
{
    private readonly IEscrowService _escrowService;

    public HomeController(IEscrowService escrowService)
    {
        _escrowService = escrowService;
    }

    public IActionResult Index()
    {
        ViewBag.IsDemoMode = _escrowService.IsDemoMode;
        return View();
    }

    public IActionResult Dashboard()
    {
        var model = new DashboardViewModel
        {
            IsDemoMode = _escrowService.IsDemoMode
        };
        return View(model);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var escrow = await _escrowService.GetEscrowAsync(id);
        if (escrow == null) return NotFound();

        var model = new EscrowViewModel { Escrow = escrow };
        ViewBag.IsDemoMode = _escrowService.IsDemoMode;
        return View(model);
    }
}
