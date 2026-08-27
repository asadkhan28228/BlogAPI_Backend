using BlogApi.BLL.Dtos.Comment;

namespace BlogApi.BLL.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllAsync();

        Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId);

        Task<CommentDto?> GetByIdAsync(int id);

        Task<CommentDto> CreateAsync(
            CreateCommentDto dto,
            int userId);

        Task<bool> UpdateAsync(
            int id,
            UpdateCommentDto dto,
            int userId);

        Task<bool> DeleteAsync(
            int id,
            int userId);
    }
}