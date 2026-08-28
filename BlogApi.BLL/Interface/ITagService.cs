using BlogApi.BLL.DTOs.Tag;

namespace BlogApi.BLL.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllAsync();

        Task<TagDto?> GetByIdAsync(int id);

        Task<TagDto> CreateAsync(CreateTagDto dto);

        Task<bool> UpdateAsync(int id,UpdateTagDto dto);

        Task<bool> DeleteAsync(int id);
    }
}