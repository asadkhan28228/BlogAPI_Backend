using BlogApi.BLL.DTOs.Category;
using BlogApi.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid category ID.");
            }

            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            try
            {
                var category = await _categoryService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = category.Id },
                    category
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid category ID.");
            }

            var result = await _categoryService.UpdateAsync(id, dto);

            if (!result)
            {
                return BadRequest("Category not found or name already exists.");
            }

            return Ok("Category updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound("Category not found.");
                }

                return Ok("Category deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}