using BlogApi.BLL.Dtos.Auth;
using BlogApi.BLL.Dtos.Auth.BlogAPI.BLL.DTOs;
using BlogApi.BLL.Interface;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Repositories;
using EMSBLL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.BLL.Services
{
    public class AuthService : IAuthService
    {
       private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        // 1. Yahan interface use karein aur sahi naam rakhein
        private readonly IJwtService _jwtServices;

        // 2. Constructor mein bhi IJwtServices inject karein
        public AuthService(IUserRepository userRepository, IJwtService jwtServices)
        {
            _userRepository = userRepository;
            _jwtServices = jwtServices;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Business rule: email already registered na ho
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already registered.");
            }
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = "User"
            };
           // Password hash karna (kabhi plain text save nahi karte)
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);

            // 3. Register mein _jwtServices call karein
            var token = _jwtServices.GenerateToken(user);

            return new AuthResponseDto
            {
                Message = "Register successfully."
                
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // 4. Login mein bhi _tokenGenerator ki jagah _jwtServices use karein
            var token = _jwtServices.GenerateToken(user);

            return new AuthResponseDto
            {
                Message = "Login successfully.",
                UserId = user.User_id,
                Email = user.Email,
                Username = user.Username,
                Token = token
            };
        }
    }
}
