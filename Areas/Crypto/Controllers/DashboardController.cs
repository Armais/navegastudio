using Microsoft.AspNetCore.Mvc;

namespace NavegaStudio.Areas.Crypto.Controllers;

[Area("Crypto")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
