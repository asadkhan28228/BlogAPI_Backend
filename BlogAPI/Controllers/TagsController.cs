using BlogApi.BLL.DTOs.Tag;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(
            ITagService tagService)
        {
            _tagService = tagService;
        }

        // =========================
        // GET ALL TAGS
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags =
                await _tagService.GetAllAsync();

            return Ok(tags);
        }

        // =========================
        // GET TAG BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid tag ID.");
            }

            var tag =
                await _tagService.GetByIdAsync(id);

            if (tag == null)
            {
                return NotFound(
                    "Tag not found.");
            }

            return Ok(tag);
        }

        // =========================
        // CREATE TAG
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTagDto dto)
        {
            if (dto == null)
            {
                return BadRequest(
                    "Tag data is required.");
            }

            try
            {
                var tag =
                    await _tagService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = tag.Id },
                    tag);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // UPDATE TAG
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTagDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid tag ID.");
            }

            if (dto == null)
            {
                return BadRequest(
                    "Tag data is required.");
            }

            var result =
                await _tagService.UpdateAsync(
                    id,
                    dto);

            if (!result)
            {
                return BadRequest(
                    "Tag not found or tag name already exists.");
            }

            return Ok(
                "Tag updated successfully.");
        }

        // =========================
        // DELETE TAG
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    "Invalid tag ID.");
            }

            var result =
                await _tagService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(
                    "Tag not found.");
            }

            return Ok(
                "Tag deleted successfully.");
        }
    }
}