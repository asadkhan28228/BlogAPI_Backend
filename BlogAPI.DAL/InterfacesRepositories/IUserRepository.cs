using BlogApi.DAL.Entities;
using System.Threading.Tasks;

namespace BlogApi.DAL.InterfacesRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUsernameAsync(string username);

        Task<bool> EmailExistsAsync(string email);

        Task<User> AddAsync(User user);
    }
}