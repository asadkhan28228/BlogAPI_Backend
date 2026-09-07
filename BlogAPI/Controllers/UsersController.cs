using BlogApi.BLL.Common;
using BlogApi.BLL.DTOs.User;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)] // Poora controller sirf Admin ke liye
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // =========================
        // GET ALL USERS (Admin only)
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        // =========================
        // GET USER BY ID (Admin only)
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // =========================
        // UPDATE USER ROLE (Admin only)
        // =========================
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId != null &&
                int.TryParse(currentUserId, out var currentUserIdValue) &&
                currentUserIdValue == id)
            {
                return BadRequest("You cannot change your own role.");
            }

            var result = await _userService.UpdateRoleAsync(id, dto);

            if (!result)
            {
                return NotFound("User not found or invalid role.");
            }

            return Ok("User role updated successfully.");
        }
    }
}