
﻿using BlogApi.BLL.Common;
using BlogApi.BLL.DTOs.Comment;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(
            ICommentService commentService)
        {
            _commentService = commentService;
        }

        // ==========================================================
        // GET ALL
        // Public
        // ==========================================================

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var comments =
                await _commentService.GetAllAsync();

            return Ok(comments);
        }

        // ==========================================================
        // GET BY POST
        // Public
        // ==========================================================

        [HttpGet("post/{postId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByPostId(
            int postId)
        {
            if (postId <= 0)
            {
                return BadRequest(
                    "Invalid post ID.");
            }

            var comments =
                await _commentService
                    .GetByPostIdAsync(postId);

            return Ok(comments);
        }

        // ==========================================================
        // GET BY ID
        // Public
        // ==========================================================

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid comment ID.");
            }

            var comment =
                await _commentService
                    .GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound(
                    "Comment not found.");
            }

            return Ok(comment);
        }

        // ==========================================================
        // CREATE
        // Login required
        // ==========================================================

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(
            [FromBody] CreateCommentDto dto)
        {
            if (dto == null)
            {
                return BadRequest(
                    "Comment data is required.");
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
                var comment =
                    await _commentService.CreateAsync(
                        dto,
                        userId.Value);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id = comment.Id
                    },
                    comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ==========================================================
        // UPDATE
        // Login required
        // ==========================================================

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateCommentDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid comment ID.");
            }

            if (dto == null)
            {
                return BadRequest(
                    "Comment data is required.");
            }

            var userId =
                GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            var result =
                await _commentService.UpdateAsync(
                    id,
                    dto,
                    userId.Value);

            if (!result)
            {
                return NotFound(
                    "Comment not found or you are not the owner.");
            }

            return Ok(
                "Comment updated successfully.");
        }

        // ==========================================================
        // DELETE
        // Login required
        // ==========================================================

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid comment ID.");
            }

            var userId =
                GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(
                    "User ID not found.");
            }

            var isAdmin =
                User.IsInRole(Roles.Admin);

            var result =
                await _commentService.DeleteAsync(
                    id,
                    userId.Value,
                    isAdmin);

            if (!result)
            {
                return NotFound(
                    "Comment not found or you are not the owner.");
            }

            return Ok(
                "Comment deleted successfully.");
        }

        // ==========================================================
        // CURRENT USER ID
        // ==========================================================

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
                out var userIdValue))
            {
                return null;
            }

            return userIdValue > 0
                ? userIdValue
                : null;
        }
    }
}

