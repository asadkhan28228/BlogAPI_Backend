using BlogApi.BLL.Common;
using BlogApi.BLL.DTOs.Post;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(
            IPostService postService)
        {
            _postService = postService;
        }

        // ==========================================
        // GET ALL POSTS
        // PUBLIC
        // ==========================================

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result =
                await _postService.GetPagedAsync(
                    page,
                    pageSize,
                    keyword,
                    categoryId);

            return Ok(result);
        }

        // ==========================================
        // GET POST BY ID
        // PUBLIC
        // ==========================================

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid post ID.");
            }

            var post =
                await _postService
                    .GetByIdAsync(id);

            if (post == null)
            {
                return NotFound(
                    "Published post not found.");
            }

            return Ok(post);
        }

        // ==========================================
        // GET POST BY SLUG
        // PUBLIC
        // ==========================================

        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(
            string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return BadRequest(
                    "Slug is required.");
            }

            var post =
                await _postService
                    .GetBySlugAsync(slug);

            if (post == null)
            {
                return NotFound(
                    "Published post not found.");
            }

            return Ok(post);
        }

        // ==========================================
        // CREATE
        // LOGIN REQUIRED
        // ==========================================

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(
            [FromBody] CreatePostDto dto)
        {
            if (dto == null)
            {
                return BadRequest(
                    "Post data is required.");
            }

            var userId =
                GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            try
            {
                var post =
                    await _postService
                        .CreateAsync(
                            dto,
                            userId.Value);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = post.Id },
                    post);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    ex.Message);
            }
        }

        // ==========================================
        // UPDATE
        // OWNER / ADMIN
        // ==========================================

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdatePostDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid post ID.");
            }

            if (dto == null)
            {
                return BadRequest(
                    "Post data is required.");
            }

            var userId =
                GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            var isAdmin =
                User.IsInRole(
                    Roles.Admin);

            var result =
                await _postService.UpdateAsync(
                    id,
                    dto,
                    userId.Value,
                    isAdmin);

            if (!result)
            {
                return NotFound(
                    "Post not found, category not found, " +
                    "or you are not the owner.");
            }

            return Ok(
                "Post updated successfully.");
        }

        // ==========================================
        // DELETE
        // OWNER / ADMIN
        // ==========================================

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid post ID.");
            }

            var userId =
                GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            var isAdmin =
                User.IsInRole(
                    Roles.Admin);

            var result =
                await _postService.DeleteAsync(
                    id,
                    userId.Value,
                    isAdmin);

            if (!result)
            {
                return NotFound(
                    "Post not found or " +
                    "you are not the owner.");
            }

            return Ok(
                "Post deleted successfully.");
        }

        // ==========================================
        // CURRENT USER ID
        // ==========================================

        private int? GetCurrentUserId()
        {
            var value =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                value,
                out var userId))
            {
                return null;
            }

            if (userId <= 0)
            {
                return null;
            }

            return userId;
        }
    }
}