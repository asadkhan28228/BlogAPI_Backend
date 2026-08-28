using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.DAL.Repositories
{
    public class PostTagRepository : IPostTagRepository
    {
        private readonly AppDbContext _context;

        public PostTagRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET TAGS OF POST
        // =========================
        public async Task<IEnumerable<PostTag>> GetByPostIdAsync(
            int postId)
        {
            return await _context.PostTags
                .Include(pt => pt.Tag)
                .Where(pt => pt.PostId == postId)
                .ToListAsync();
        }

        // =========================
        // GET POST TAG
        // =========================
        public async Task<PostTag?> GetAsync(
            int postId,
            int tagId)
        {
            return await _context.PostTags
                .FirstOrDefaultAsync(pt =>
                    pt.PostId == postId &&
                    pt.TagId == tagId);
        }

        // =========================
        // ADD TAG TO POST
        // =========================
        public async Task<PostTag> AddAsync(
            PostTag postTag)
        {
            await _context.PostTags.AddAsync(postTag);

            await _context.SaveChangesAsync();

            return postTag;
        }

        // =========================
        // REMOVE TAG FROM POST
        // =========================
        public async Task DeleteAsync(
            int postId,
            int tagId)
        {
            var postTag =
                await _context.PostTags
                    .FirstOrDefaultAsync(pt =>
                        pt.PostId == postId &&
                        pt.TagId == tagId);

            if (postTag == null)
            {
                return;
            }

            _context.PostTags.Remove(postTag);

            await _context.SaveChangesAsync();
        }
    }
}