using BlogApi.BLL.Dtos.Auth;

namespace BlogApi.BLL.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        Task<AuthResponseDto?> RefreshTokenAsync(
            RefreshTokenRequestDto dto);

        Task<bool> RevokeRefreshTokenAsync(
            string refreshToken);
    }
}