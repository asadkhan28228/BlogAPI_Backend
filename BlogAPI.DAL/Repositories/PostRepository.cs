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

        // =========================
        // GET ALL POSTS
        // =========================
        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // GET POST BY ID
        // =========================
        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // =========================
        // SEARCH POSTS
        // =========================
        public async Task<IEnumerable<Post>> SearchAsync(
            string keyword)
        {
            return await _context.Posts
                .Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Content.Contains(keyword))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // TOTAL POSTS COUNT
        // =========================
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Posts.CountAsync();
        }

        // =========================
        // SEARCH COUNT
        // =========================
        public async Task<int> GetSearchCountAsync(
            string keyword)
        {
            return await _context.Posts
                .Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Content.Contains(keyword))
                .CountAsync();
        }

        // =========================
        // PAGED POSTS
        // =========================
        public async Task<IEnumerable<Post>> GetPagedAsync(
            int page,
            int pageSize)
        {
            return await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // =========================
        // SEARCH + PAGINATION
        // =========================
        public async Task<IEnumerable<Post>> SearchPagedAsync(
            string keyword,
            int page,
            int pageSize)
        {
            return await _context.Posts
                .Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Content.Contains(keyword))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // =================================================
        // FILTER + SEARCH COUNT
        // =================================================
        public async Task<int> GetFilteredCountAsync(
            string? keyword,
            int? categoryId)
        {
            var query = _context.Posts
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Content.Contains(keyword));
            }

            // CATEGORY FILTER
            if (categoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == categoryId.Value);
            }

            return await query.CountAsync();
        }

        // =================================================
        // FILTER + SEARCH + PAGINATION
        // =================================================
        public async Task<IEnumerable<Post>> GetFilteredPagedAsync(
            string? keyword,
            int? categoryId,
            int page,
            int pageSize)
        {
            var query = _context.Posts
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Content.Contains(keyword));
            }

            // CATEGORY FILTER
            if (categoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == categoryId.Value);
            }

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // =========================
        // CREATE
        // =========================
        public async Task<Post> AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);

            await _context.SaveChangesAsync();

            return post;
        }

        // =========================
        // UPDATE
        // =========================
        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);

            await _context.SaveChangesAsync();
        }

        // =========================
        // DELETE
        // =========================
        public async Task DeleteAsync(int id)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return;
            }

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();
        }
    }
}