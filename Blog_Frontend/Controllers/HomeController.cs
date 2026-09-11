using BlogMVC.Models.Posts;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result =
                    await _apiService
                        .GetAsync<PostPagedResultViewModel>(
                            "api/Posts?page=1&pageSize=6");

                return View(result ??
                    new PostPagedResultViewModel());
            }
            catch
            {
                return View(
                    new PostPagedResultViewModel());
            }
        }
    }
}