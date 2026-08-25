using BlogApi.BLL.Dtos.Auth;
using BlogApi.BLL.Dtos.Auth.BlogAPI.BLL.DTOs;
using BlogApi.BLL.Interface;
using BlogApi.BLL.Service;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        
        private readonly PasswordHasher<User> _passwordHasher;

        private readonly JwtServices _Jwtsevice;

        public AuthService(IUserRepository userRepository,JwtServices jwtsevice)
        {
            _userRepository = userRepository;
            _Jwtsevice = jwtsevice;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Business rule: email already registered na ho
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email already registered.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = "User"
            };

            // Password hash karna (kabhi plain text save nahi karte)
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);

            var token = _JwtServices.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid email or password.");

            var token = _tokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token
            };
        }
    }
}