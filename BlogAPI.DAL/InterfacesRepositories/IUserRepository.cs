using BlogApi.DAL.Entities;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int User_id);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUsernameAsync(string username);

        Task<bool> EmailExistsAsync(string email);

        Task<User> AddAsync(User user);

        Task<IEnumerable<User>> GetAllAsync();

        Task UpdateAsync(User user);
    }
}