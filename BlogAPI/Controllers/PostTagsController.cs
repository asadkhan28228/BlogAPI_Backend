using BlogApi.BLL.DTOs.PostTag;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostTagsController : ControllerBase
    {
        private readonly IPostTagService _postTagService;

        public PostTagsController(
            IPostTagService postTagService)
        {
            _postTagService = postTagService;
        }

        // =========================
        // GET TAGS OF POST
        // =========================
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPostId(
            int postId)
        {
            if (postId <= 0)
            {
                return BadRequest(
                    "Invalid post ID.");
            }

            var tags =
                await _postTagService
                    .GetByPostIdAsync(postId);

            return Ok(tags);
        }

        // =========================
        // ADD TAG TO POST
        // =========================
        [HttpPost("post/{postId}")]
        public async Task<IActionResult> AddTag(
            int postId,
            [FromBody] AddPostTagDto dto)
        {
            if (postId <= 0)
            {
                return BadRequest(
                    "Invalid post ID.");
            }

            if (dto == null)
            {
                return BadRequest(
                    "Tag data is required.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            try
            {
                var result =
                    await _postTagService
                        .AddTagToPostAsync(
                            postId,
                            dto,
                            userId.Value);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // =========================
        // REMOVE TAG FROM POST
        // =========================
        [HttpDelete("post/{postId}/tag/{tagId}")]
        public async Task<IActionResult> RemoveTag(
            int postId,
            int tagId)
        {
            if (postId <= 0 ||
                tagId <= 0)
            {
                return BadRequest(
                    "Invalid post ID or tag ID.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            var result =
                await _postTagService
                    .RemoveTagFromPostAsync(
                        postId,
                        tagId,
                        userId.Value);

            if (!result)
            {
                return NotFound(
                    "Post, tag assignment not found, or you are not the owner.");
            }

            return Ok(
                "Tag removed from post successfully.");
        }

        // =========================
        // GET USER ID FROM JWT
        // =========================
        private int? GetCurrentUserId()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            if (!int.TryParse(
                    userId,
                    out int userIdValue))
            {
                return null;
            }

            if (userIdValue <= 0)
            {
                return null;
            }

            return userIdValue;
        }
    }
}