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
                .Include(p => p.User)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // GET POST BY ID
        // =========================

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // =========================
        // GET POST BY SLUG
        // =========================

        public async Task<Post?> GetBySlugAsync(string slug)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        // =========================
        // CHECK SLUG
        // =========================

        public async Task<bool> ExistsBySlugAsync(
            string slug,
            int? excludePostId = null)
        {
            var query = _context.Posts
                .Where(p => p.Slug == slug);

            if (excludePostId.HasValue)
            {
                query = query.Where(
                    p => p.Id != excludePostId.Value);
            }

            return await query.AnyAsync();
        }

        // =========================
        // SEARCH POSTS
        // =========================

        public async Task<IEnumerable<Post>> SearchAsync(
            string keyword)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Where(p =>
                    p.IsPublished &&
                    (p.Title.Contains(keyword) ||
                     p.Content.Contains(keyword)))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // TOTAL POSTS COUNT
        // =========================

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Posts
                .Where(p => p.IsPublished)
                .CountAsync();
        }

        // =========================
        // SEARCH COUNT
        // =========================

        public async Task<int> GetSearchCountAsync(
            string keyword)
        {
            return await _context.Posts
                .Where(p =>
                    p.IsPublished &&
                    (p.Title.Contains(keyword) ||
                     p.Content.Contains(keyword)))
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
                .Include(p => p.User)
                .Include(p => p.Category)
                .Where(p => p.IsPublished)
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
                .Include(p => p.User)
                .Include(p => p.Category)
                .Where(p =>
                    p.IsPublished &&
                    (p.Title.Contains(keyword) ||
                     p.Content.Contains(keyword)))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // =========================
        // FILTER COUNT
        // =========================

        public async Task<int> GetFilteredCountAsync(
            string? keyword,
            int? categoryId,
            bool publishedOnly = true)
        {
            var query = _context.Posts
                .AsQueryable();

            if (publishedOnly)
            {
                query = query.Where(
                    p => p.IsPublished);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Content.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(
                    p => p.CategoryId == categoryId.Value);
            }

            return await query.CountAsync();
        }

        // =========================
        // FILTER + PAGINATION
        // =========================

        public async Task<IEnumerable<Post>> GetFilteredPagedAsync(
            string? keyword,
            int? categoryId,
            int page,
            int pageSize,
            bool publishedOnly = true)
        {
            var query = _context.Posts
                .AsQueryable();

            if (publishedOnly)
            {
                query = query.Where(
                    p => p.IsPublished);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Content.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(
                    p => p.CategoryId == categoryId.Value);
            }

            return await query
                .Include(p => p.User)
                .Include(p => p.Category)
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