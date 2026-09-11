using BlogMVC.Models.Categories;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;

        public CategoriesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categories =
                    await _apiService
                        .GetAsync<List<CategoryViewModel>>(
                            "api/Categories");

                return View(
                    categories ??
                    new List<CategoryViewModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(
                    new List<CategoryViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.PostAsync<CategoryViewModel>(
                    "api/Categories",
                    new
                    {
                        name = model.Name,
                        description = model.Description
                    });

                TempData["Success"] =
                    "Category created successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message);

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category =
                    await _apiService
                        .GetAsync<CategoryViewModel>(
                            $"api/Categories/{id}");

                if (category == null)
                {
                    return NotFound();
                }

                return View(
                    new CategoryFormViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.PutAsync<string>(
                    $"api/Categories/{model.Id}",
                    new
                    {
                        name = model.Name,
                        description = model.Description
                    });

                TempData["Success"] =
                    "Category updated successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiService.DeleteAsync<string>(
                    $"api/Categories/{id}");

                TempData["Success"] =
                    "Category deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}