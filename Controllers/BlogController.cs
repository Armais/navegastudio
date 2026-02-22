using Microsoft.AspNetCore.Mvc;
using NavegaStudio.Services;

namespace NavegaStudio.Controllers;

public class BlogController : Controller
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public IActionResult Index()
    {
        var posts = _blogService.GetAll();
        return View(posts);
    }

    [Route("Blog/Post/{slug}")]
    public IActionResult Post(string slug)
    {
        var post = _blogService.GetBySlug(slug);
        if (post == null)
            return NotFound();

        return View(post);
    }
}
