using BlogApi.DAL.Entities;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllAsync();

        Task<Post?> GetByIdAsync(int id);

        Task<Post?> GetBySlugAsync(string slug);

        Task<bool> ExistsBySlugAsync(
            string slug,
            int? excludePostId = null);

        Task<IEnumerable<Post>> SearchAsync(
            string keyword);

        Task<int> GetTotalCountAsync();

        Task<int> GetSearchCountAsync(
            string keyword);

        Task<IEnumerable<Post>> GetPagedAsync(
            int page,
            int pageSize);

        Task<IEnumerable<Post>> SearchPagedAsync(
            string keyword,
            int page,
            int pageSize);

        Task<int> GetFilteredCountAsync(
            string? keyword,
            int? categoryId,
            bool publishedOnly = true);

        Task<IEnumerable<Post>> GetFilteredPagedAsync(
            string? keyword,
            int? categoryId,
            int page,
            int pageSize,
            bool publishedOnly = true);

        Task<Post> AddAsync(Post post);

        Task UpdateAsync(Post post);

        Task DeleteAsync(int id);
    }
}