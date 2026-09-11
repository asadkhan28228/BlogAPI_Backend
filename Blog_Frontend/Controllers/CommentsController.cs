using Microsoft.AspNetCore.Mvc;

namespace Blog_Frontend.Controllers
{
    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
