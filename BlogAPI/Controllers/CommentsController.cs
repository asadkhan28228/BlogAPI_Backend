using BlogApi.BLL.DTOs.Comment;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(
            ICommentService commentService)
        {
            _commentService = commentService;
        }

        // =========================
        // GET ALL COMMENTS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments =await _commentService.GetAllAsync();

            return Ok(comments);
        }

        // =========================
        // GET COMMENTS BY POST
        // =========================
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPostId(int postId)
        {
            if (postId <= 0)
            {
                return BadRequest("Invalid post ID.");
            }

            var comments =
                await _commentService.GetByPostIdAsync(postId);
                return Ok(comments);
        }

        // =========================
        // GET COMMENT BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid comment ID.");
            }

            var comment =await _commentService.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            return Ok(comment);
        }

        // =========================
        // CREATE COMMENT
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Comment data is required.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("User ID not found.");
            }

            try
            {
                var comment =await _commentService.CreateAsync(dto,userId.Value);

                return CreatedAtAction(nameof(GetById),new { id = comment.Id },comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // UPDATE COMMENT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] UpdateCommentDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid comment ID.");
            }

            if (dto == null)
            {
                return BadRequest("Comment data is required.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("User ID not found.");
            }

            var result =await _commentService.UpdateAsync(id,dto,userId.Value);

            if (!result)
            {
                return NotFound("Comment not found or you are not the owner.");
            }

            return Ok("Comment updated successfully.");
        }

        // =========================
        // DELETE COMMENT
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid comment ID.");
            }

            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            var result =
                await _commentService.DeleteAsync(
                    id,
                    userId.Value);

            if (!result)
            {
                return NotFound(
                    "Comment not found or you are not the owner.");
            }

            return Ok(
                "Comment deleted successfully.");
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