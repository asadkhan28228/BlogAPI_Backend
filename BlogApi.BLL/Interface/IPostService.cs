using BlogApi.BLL.DTOs.Post;

namespace BlogApi.BLL.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAllAsync();

        Task<PostDto?> GetByIdAsync(int id);

        Task<IEnumerable<PostDto>> SearchAsync(
            string keyword);

        Task<PostPagedResultDto> GetPagedAsync(
            int page,
            int pageSize,
            string? keyword,
            int? categoryId);

        Task<PostDto> CreateAsync(
            CreatePostDto dto,
            int userId);

        Task<bool> UpdateAsync(
            int id,
            UpdatePostDto dto,
            int userId);

        Task<bool> DeleteAsync(
            int id,
            int userId);
    }
}