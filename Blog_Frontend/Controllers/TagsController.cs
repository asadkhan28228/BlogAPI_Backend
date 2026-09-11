using Microsoft.AspNetCore.Mvc;

namespace Blog_Frontend.Controllers
{
    public class TagsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
