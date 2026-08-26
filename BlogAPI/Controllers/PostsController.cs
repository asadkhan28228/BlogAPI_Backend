using BlogApi.BLL.DTOs.Post;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllAsync();

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post == null)
                return NotFound("Post not found.");

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var post = await _postService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = post.Id },
                post
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdatePostDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _postService.UpdateAsync(
                id,
                dto,
                userId
            );

            if (!result)
                return NotFound("Post not found or you are not the owner.");

            return Ok("Post updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var result = await _postService.DeleteAsync(
                id,
                userId
            );

            if (!result)
                return NotFound("Post not found or you are not the owner.");

            return Ok("Post deleted successfully.");
        }
    }
}