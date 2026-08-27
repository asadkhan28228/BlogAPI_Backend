using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.DAL.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all comments
        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // Get comments of a specific post
        public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // Get comment by ID
        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Create comment
        public async Task<Comment> AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);

            await _context.SaveChangesAsync();

            return comment;
        }

        // Update comment
        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);

            await _context.SaveChangesAsync();
        }

        // Delete comment
        public async Task DeleteAsync(int id)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return;
            }

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();
        }
    }
}