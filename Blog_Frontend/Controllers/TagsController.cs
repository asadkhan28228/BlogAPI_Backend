using BlogMVC.Models.Tags;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApiService _apiService;

        public TagsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var tags =
                    await _apiService
                        .GetAsync<List<TagViewModel>>(
                            "api/Tags");

                return View(
                    tags ??
                    new List<TagViewModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(
                    new List<TagViewModel>());
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
            TagFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.PostAsync<TagViewModel>(
                    "api/Tags",
                    new
                    {
                        name = model.Name
                    });

                TempData["Success"] =
                    "Tag created successfully.";

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
                var tag =
                    await _apiService
                        .GetAsync<TagViewModel>(
                            $"api/Tags/{id}");

                if (tag == null)
                {
                    return NotFound();
                }

                return View(
                    new TagFormViewModel
                    {
                        Id = tag.Id,
                        Name = tag.Name
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
            TagFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.PutAsync<string>(
                    $"api/Tags/{model.Id}",
                    new
                    {
                        name = model.Name
                    });

                TempData["Success"] =
                    "Tag updated successfully.";

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
                    $"api/Tags/{id}");

                TempData["Success"] =
                    "Tag deleted successfully.";
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