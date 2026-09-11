namespace BlogMVC.Models.Auth
{
    public class AuthResponseModel
    {
        public string Message { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        // Backend se User / Admin receive hoga
        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresAt { get; set; }

        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}

