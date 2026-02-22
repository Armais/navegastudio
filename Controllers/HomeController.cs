using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NavegaStudio.Models;
using NavegaStudio.Models.Shared;
using NavegaStudio.Services;

namespace NavegaStudio.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IEmailService emailService, ILogger<HomeController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var notified = await _emailService.SendContactNotificationAsync(model);
        _logger.LogInformation("Contact form from {Name} ({Email}), subject: {Subject}, notification sent: {Sent}",
            model.Name, model.Email, model.Subject, notified);

        var confirmed = await _emailService.SendContactConfirmationAsync(model);
        if (confirmed)
            _logger.LogInformation("Confirmation email sent to {Email}", model.Email);

        TempData["ContactSuccess"] = true;
        return RedirectToAction(nameof(Contact));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
