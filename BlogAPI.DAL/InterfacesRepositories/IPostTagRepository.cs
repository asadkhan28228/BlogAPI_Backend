using BlogApi.DAL.Entities;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface IPostTagRepository
    {
        Task<IEnumerable<PostTag>> GetByPostIdAsync(int postId);

        Task<PostTag?> GetAsync(
            int postId,
            int tagId);

        Task<PostTag> AddAsync(PostTag postTag);

        Task DeleteAsync(
            int postId,
            int tagId);
    }
}