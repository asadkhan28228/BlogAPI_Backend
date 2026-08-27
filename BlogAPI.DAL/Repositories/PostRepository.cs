
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.DAL.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts.ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Post> AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}