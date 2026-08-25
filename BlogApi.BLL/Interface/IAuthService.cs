using BlogApi.BLL.Dtos.Auth;
using BlogApi.BLL.Dtos.Auth.BlogAPI.BLL.DTOs;

namespace BlogApi.BLL.Interface
{   
        public interface IAuthService
        {
            Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
            Task<AuthResponseDto> LoginAsync(LoginDto dto);
        }
    
}
