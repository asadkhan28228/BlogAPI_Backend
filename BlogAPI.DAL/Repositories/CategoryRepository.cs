using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.DAL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasPostsAsync(int categoryId)
        {
            return await _context.Posts.AnyAsync(p => p.CategoryId == categoryId);
        }
    }
}