using Microsoft.AspNetCore.Mvc;

namespace Blog_Frontend.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
