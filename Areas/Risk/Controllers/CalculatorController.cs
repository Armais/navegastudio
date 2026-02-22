using Microsoft.AspNetCore.Mvc;

namespace NavegaStudio.Areas.Risk.Controllers;

[Area("Risk")]
public class CalculatorController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Trading Risk Calculator";
        return View();
    }
}
