using BlogApi.DAL.Entities;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();

        Task<Tag?> GetByIdAsync(int id);

        Task<Tag?> GetByNameAsync(string name);

        Task<Tag> AddAsync(Tag tag);

        Task UpdateAsync(Tag tag);

        Task DeleteAsync(int id);
    }
}