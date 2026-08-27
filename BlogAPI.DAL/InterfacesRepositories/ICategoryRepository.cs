using BlogApi.DAL.Entities;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<bool> ExistsByNameAsync(string name);

        Task<Category> AddAsync(Category category);

        Task UpdateAsync(Category category);

        Task DeleteAsync(int id);

        Task<bool> HasPostsAsync(int categoryId);
    }
}