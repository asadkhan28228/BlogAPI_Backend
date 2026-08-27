using BlogApi.BLL.DTOs.Category;
using BlogApi.BLL.Interfaces;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;

namespace BlogApi.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            if (categories == null || !categories.Any())
            {
                return Enumerable.Empty<CategoryDto>();
            }

            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return MapToDto(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Category name is required.");
            }

            var nameExists = await _categoryRepository.ExistsByNameAsync(dto.Name.Trim());

            if (nameExists)
            {
                throw new ArgumentException("A category with this name already exists.");
            }

            var category = new Category
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim()
            };

            var createdCategory = await _categoryRepository.AddAsync(category);

            return MapToDto(createdCategory);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            if (id <= 0)
            {
                return false;
            }

            if (dto == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return false;
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            var nameExists = await _categoryRepository.ExistsByNameAsync(dto.Name.Trim());

            if (nameExists && !category.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            category.Name = dto.Name.Trim();
            category.Description = dto.Description?.Trim();

            await _categoryRepository.UpdateAsync(category);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            var hasPosts = await _categoryRepository.HasPostsAsync(id);

            if (hasPosts)
            {
                throw new InvalidOperationException(
                    "Cannot delete category because it has posts associated with it."
                );
            }

            await _categoryRepository.DeleteAsync(id);

            return true;
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}