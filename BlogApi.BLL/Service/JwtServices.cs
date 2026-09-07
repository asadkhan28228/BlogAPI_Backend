using BlogApi.BLL.Interface;
using BlogApi.DAL.Entities;
using EMSBLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EMSBLL.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // ============================================
        // Generate Access Token
        // ============================================

        public string GenerateToken(User user)
        {
            var key = _configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "JWT SecretKey is missing from configuration.");
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.User_id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };


            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key));


            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);


            var expiryMinutes =
                Convert.ToDouble(
                    _configuration["JwtSettings:ExpiryInMinutes"]);


            var token = new JwtSecurityToken(

                issuer:
                    _configuration["JwtSettings:Issuer"],

                audience:
                    _configuration["JwtSettings:Audience"],

                claims: claims,

                expires:
                    DateTime.UtcNow.AddMinutes(expiryMinutes),

                signingCredentials:
                    credentials
            );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }


        // ============================================
        // Access Token Expiration
        // ============================================

        public DateTime GetTokenExpiration()
        {
            var expiryMinutes =
                Convert.ToDouble(
                    _configuration["JwtSettings:ExpiryInMinutes"]);

            return DateTime.UtcNow.AddMinutes(expiryMinutes);
        }


        // ============================================
        // Generate Secure Refresh Token
        // ============================================

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}