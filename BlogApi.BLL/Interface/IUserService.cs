using BlogApi.BLL.DTOs.User;

namespace BlogApi.BLL.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(int id);

        Task<bool> UpdateRoleAsync(int id, UpdateUserRoleDto dto);
    }
}