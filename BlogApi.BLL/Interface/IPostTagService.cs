using BlogApi.BLL.DTOs.PostTag;

namespace BlogApi.BLL.Interfaces
{
    public interface IPostTagService
    {
        Task<IEnumerable<PostTagDto>> GetByPostIdAsync(
            int postId);

        Task<PostTagDto> AddTagToPostAsync(
            int postId,
            AddPostTagDto dto,
            int userId);

        Task<bool> RemoveTagFromPostAsync(
            int postId,
            int tagId,
            int userId);
    }
}