using BlogMVC.Models.Comments;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApiService _apiService;

        public CommentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateCommentViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(
                    "Details",
                    "Posts",
                    new { id = model.PostId });
            }

            try
            {
                await _apiService.PostAsync<CommentViewModel>(
                    "api/Comments",
                    new
                    {
                        postId = model.PostId,
                        content = model.Content
                    });

                TempData["Success"] =
                    "Comment added successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(
                "Details",
                "Posts",
                new { id = model.PostId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id,
            int postId)
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
                    $"api/Comments/{id}");

                TempData["Success"] =
                    "Comment deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(
                "Details",
                "Posts",
                new { id = postId });
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(
                    "AccessToken"));
        }
    }
}