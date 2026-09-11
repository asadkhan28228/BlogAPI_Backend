using Microsoft.AspNetCore.Mvc;

namespace Blog_Frontend.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
