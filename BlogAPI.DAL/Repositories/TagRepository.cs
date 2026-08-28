using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.DAL.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL TAGS
        // =========================
        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        // =========================
        // GET TAG BY ID
        // =========================
        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // =========================
        // GET TAG BY NAME
        // =========================
        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t =>
                    t.Name.ToLower() == name.ToLower());
        }

        // =========================
        // CREATE TAG
        // =========================
        public async Task<Tag> AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);

            await _context.SaveChangesAsync();

            return tag;
        }

        // =========================
        // UPDATE TAG
        // =========================
        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);

            await _context.SaveChangesAsync();
        }

        // =========================
        // DELETE TAG
        // =========================
        public async Task DeleteAsync(int id)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
            {
                return;
            }

            _context.Tags.Remove(tag);

            await _context.SaveChangesAsync();
        }
    }
}