using BlogMVC.Models.Categories;
using BlogMVC.Models.Comments;
using BlogMVC.Models.Posts;
using BlogMVC.Models.Tags;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApiService _apiService;

        public PostsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ==========================================================
        // LIST
        // ==========================================================

        public async Task<IActionResult> Index(
            string? keyword,
            int? categoryId,
            int page = 1)
        {
            try
            {
                var endpoint =
                    $"api/Posts?page={page}&pageSize=10";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    endpoint +=
                        $"&keyword={Uri.EscapeDataString(keyword)}";
                }

                if (categoryId.HasValue)
                {
                    endpoint +=
                        $"&categoryId={categoryId.Value}";
                }

                var posts =
                    await _apiService
                        .GetAsync<PostPagedResultViewModel>(
                            endpoint);

                ViewBag.Keyword = keyword;
                ViewBag.CategoryId = categoryId;

                var categories =
                    await _apiService
                        .GetAsync<List<CategoryViewModel>>(
                            "api/Categories");

                ViewBag.Categories =
                    categories ?? new List<CategoryViewModel>();

                return View(
                    posts ??
                    new PostPagedResultViewModel());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(
                    new PostPagedResultViewModel());
            }
        }

        // ==========================================================
        // DETAILS
        // ==========================================================

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var post =
                    await _apiService
                        .GetAsync<PostViewModel>(
                            $"api/Posts/{id}");

                if (post == null)
                {
                    return NotFound();
                }

                var comments =
                    await _apiService
                        .GetAsync<List<CommentViewModel>>(
                            $"api/Comments/post/{id}");

                var tags =
                    await _apiService
                        .GetAsync<List<TagViewModel>>(
                            $"api/PostTags/post/{id}");

                ViewBag.Comments =
                    comments ?? new List<CommentViewModel>();

                ViewBag.Tags =
                    tags ?? new List<TagViewModel>();

                return View(post);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction("Index");
            }
        }

        // ==========================================================
        // CREATE GET
        // ==========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            await LoadCategories();

            return View();
        }

        // ==========================================================
        // CREATE POST
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreatePostViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories();

                return View(model);
            }

            try
            {
                var post =
                    await _apiService
                        .PostAsync<PostViewModel>(
                            "api/Posts",
                            new
                            {
                                title = model.Title,
                                content = model.Content,
                                categoryId = model.CategoryId,
                                isPublished = model.IsPublished
                            });

                TempData["Success"] =
                    "Post created successfully.";

                if (post != null)
                {
                    return RedirectToAction(
                        "Details",
                        new { id = post.Id });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message);

                await LoadCategories();

                return View(model);
            }
        }

        // ==========================================================
        // EDIT GET
        // ==========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            try
            {
                var post =
                    await _apiService
                        .GetAsync<PostViewModel>(
                            $"api/Posts/{id}");

                if (post == null)
                {
                    return NotFound();
                }

                await LoadCategories();

                return View(
                    new UpdatePostViewModel
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,
                        CategoryId = post.CategoryId,
                        IsPublished = post.IsPublished
                    });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction("Index");
            }
        }

        // ==========================================================
        // EDIT POST
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            UpdatePostViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories();

                return View(model);
            }

            try
            {
                await _apiService.PutAsync<string>(
                    $"api/Posts/{model.Id}",
                    new
                    {
                        title = model.Title,
                        content = model.Content,
                        categoryId = model.CategoryId,
                        isPublished = model.IsPublished
                    });

                TempData["Success"] =
                    "Post updated successfully.";

                return RedirectToAction(
                    "Details",
                    new { id = model.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message);

                await LoadCategories();

                return View(model);
            }
        }

        // ==========================================================
        // DELETE
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            try
            {
                await _apiService.DeleteAsync<string>(
                    $"api/Posts/{id}");

                TempData["Success"] =
                    "Post deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ==========================================================
        // LOAD CATEGORIES
        // ==========================================================

        private async Task LoadCategories()
        {
            try
            {
                var categories =
                    await _apiService
                        .GetAsync<List<CategoryViewModel>>(
                            "api/Categories");

                ViewBag.Categories =
                    categories ??
                    new List<CategoryViewModel>();
            }
            catch
            {
                ViewBag.Categories =
                    new List<CategoryViewModel>();
            }
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(
                    "AccessToken"));
        }
    }
}