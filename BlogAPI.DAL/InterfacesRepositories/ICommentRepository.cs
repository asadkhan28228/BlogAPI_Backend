using BlogApi.DAL.Entities;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync();

        Task<IEnumerable<Comment>> GetByPostIdAsync(int postId);

        Task<Comment?> GetByIdAsync(int id);

        Task<Comment> AddAsync(Comment comment);

        Task UpdateAsync(Comment comment);

        Task DeleteAsync(int id);
    }
}