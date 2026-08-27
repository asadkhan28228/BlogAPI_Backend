using BlogApi.BLL.DTOs.Post;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        // =========================
        // GET ALL POSTS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllAsync();

            return Ok(posts);
        }

        // =========================
        // GET POST BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid post ID.");
            }

            var post = await _postService.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            return Ok(post);
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePostDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Post data is required.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("User ID not found.");
            }

            try
            {
                var post = await _postService.CreateAsync(
                    dto,
                    userId.Value
                );

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = post.Id },
                    post
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // UPDATE POST
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdatePostDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid post ID.");
            }

            if (dto == null)
            {
                return BadRequest("Post data is required.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("User ID not found.");
            }

            var result = await _postService.UpdateAsync(
                id,
                dto,
                userId.Value
            );

            if (!result)
            {
                return NotFound(
                    "Post not found, category not found, or you are not the owner."
                );
            }

            return Ok("Post updated successfully.");
        }

        // =========================
        // DELETE POST
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid post ID.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("User ID not found.");
            }

            var result = await _postService.DeleteAsync(
                id,
                userId.Value
            );

            if (!result)
            {
                return NotFound(
                    "Post not found or you are not the owner."
                );
            }

            return Ok("Post deleted successfully.");
        }

        // =========================
        // GET USER ID FROM JWT
        // =========================
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

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