using BlogApi.DAL.Entities;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllAsync();

        Task<Post?> GetByIdAsync(int id);

        Task<Post> AddAsync(Post post);

        Task UpdateAsync(Post post);

        Task DeleteAsync(int id);
    }
}