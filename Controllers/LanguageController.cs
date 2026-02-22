using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace NavegaStudio.Controllers;

[Route("language")]
public class LanguageController : Controller
{
    [HttpGet("set/{culture}")]
    public IActionResult Set(string culture)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        var referer = Request.Headers.Referer.ToString();
        return Redirect(string.IsNullOrEmpty(referer) ? "/" : referer);
    }
}
