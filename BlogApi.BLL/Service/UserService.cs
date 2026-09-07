using BlogApi.BLL.Common;
using BlogApi.BLL.DTOs.User;
using BlogApi.BLL.Interfaces;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;

namespace BlogApi.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // =========================
        // GET ALL USERS
        // =========================
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(MapToDto);
        }

        // =========================
        // GET USER BY ID
        // =========================
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(id);

            return user == null ? null : MapToDto(user);
        }

        // =========================
        // UPDATE USER ROLE (Admin only - enforced at controller level)
        // =========================
        public async Task<bool> UpdateRoleAsync(int id, UpdateUserRoleDto dto)
        {
            if (id <= 0 || dto == null)
            {
                return false;
            }

            if (dto.Role != Roles.Admin && dto.Role != Roles.User)
            {
                return false;
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            user.Role = dto.Role;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.User_id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
    }
}