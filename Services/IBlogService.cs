using NavegaStudio.Models;

namespace NavegaStudio.Services;

public interface IBlogService
{
    IReadOnlyList<BlogPost> GetAll();
    BlogPost? GetBySlug(string slug);
}
