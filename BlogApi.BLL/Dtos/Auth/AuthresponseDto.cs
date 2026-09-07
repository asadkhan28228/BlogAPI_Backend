namespace BlogApi.BLL.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string Message { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresAt { get; set; }

        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}