
﻿using BlogApi.BLL.Dtos.Auth;
using BlogApi.BLL.Interface;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;
using BlogAPI.DAL.Data;
using EMSBLL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        private readonly AppDbContext _context;

        private readonly PasswordHasher<User> _passwordHasher;

        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            AppDbContext context,
            IJwtService jwtService)
        {
            _userRepository = userRepository;

            _context = context;

            _jwtService = jwtService;

            _passwordHasher =
                new PasswordHasher<User>();
        }

        // ==========================================================
        // REGISTER
        // ==========================================================

        public async Task<AuthResponseDto> RegisterAsync(
            RegisterDto dto)
        {
            var existingUser =
                await _userRepository
                    .GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new Exception(
                    "Email already registered.");
            }

            // ======================================================
            // IMPORTANT:
            // Every normal registration is ALWAYS User.
            // Client se Role receive nahi kar rahe.
            // ======================================================

            var user = new User
            {
                Username = dto.Username,

                Email = dto.Email,

                Role = "User",

                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    dto.Password);

            await _userRepository.AddAsync(user);

            // ======================================================
            // ACCESS TOKEN
            // ======================================================

            var accessToken =
                _jwtService.GenerateToken(user);

            // ======================================================
            // REFRESH TOKEN
            // ======================================================

            var refreshToken =
                CreateRefreshToken(user);

            await _context.RefreshTokens.AddAsync(
                refreshToken);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Message =
                    "Registration successful.",

                UserId =
                    user.User_id,

                Email =
                    user.Email,

                Username =
                    user.Username,

                Role =
                    user.Role,

                Token =
                    accessToken,

                RefreshToken =
                    refreshToken.Token,

                AccessTokenExpiresAt =
                    _jwtService.GetTokenExpiration(),

                RefreshTokenExpiresAt =
                    refreshToken.ExpiresAt
            };
        }

        // ==========================================================
        // LOGIN
        // ==========================================================

        public async Task<AuthResponseDto> LoginAsync(
            LoginDto dto)
        {
            var user =
                await _userRepository
                    .GetByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var result =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password);

            if (result ==
                PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            // ======================================================
            // ACCESS TOKEN
            // ======================================================

            var accessToken =
                _jwtService.GenerateToken(user);

            // ======================================================
            // REFRESH TOKEN
            // ======================================================

            var refreshToken =
                CreateRefreshToken(user);

            await _context.RefreshTokens.AddAsync(
                refreshToken);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Message =
                    "Login successful.",

                UserId =
                    user.User_id,

                Email =
                    user.Email,

                Username =
                    user.Username,

                Role =
                    user.Role,

                Token =
                    accessToken,

                RefreshToken =
                    refreshToken.Token,

                AccessTokenExpiresAt =
                    _jwtService.GetTokenExpiration(),

                RefreshTokenExpiresAt =
                    refreshToken.ExpiresAt
            };
        }

        // ==========================================================
        // REFRESH TOKEN
        // ==========================================================

        public async Task<AuthResponseDto?>
            RefreshTokenAsync(
                RefreshTokenRequestDto dto)
        {
            var refreshToken =
                await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(
                        rt => rt.Token ==
                              dto.RefreshToken);

            if (refreshToken == null)
            {
                return null;
            }

            if (refreshToken.IsRevoked)
            {
                return null;
            }

            if (refreshToken.ExpiresAt <=
                DateTime.UtcNow)
            {
                return null;
            }

            var user =
                refreshToken.User;

            // Revoke old token
            refreshToken.RevokedAt =
                DateTime.UtcNow;

            // New access token
            var newAccessToken =
                _jwtService.GenerateToken(user);

            // New refresh token
            var newRefreshToken =
                CreateRefreshToken(user);

            await _context.RefreshTokens.AddAsync(
                newRefreshToken);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Message =
                    "Token refreshed successfully.",

                UserId =
                    user.User_id,

                Email =
                    user.Email,

                Username =
                    user.Username,

                Role =
                    user.Role,

                Token =
                    newAccessToken,

                RefreshToken =
                    newRefreshToken.Token,

                AccessTokenExpiresAt =
                    _jwtService.GetTokenExpiration(),

                RefreshTokenExpiresAt =
                    newRefreshToken.ExpiresAt
            };
        }

        // ==========================================================
        // LOGOUT
        // ==========================================================

        public async Task<bool>
            RevokeRefreshTokenAsync(
                string refreshToken)
        {
            var token =
                await _context.RefreshTokens
                    .FirstOrDefaultAsync(
                        rt => rt.Token ==
                              refreshToken);

            if (token == null)
            {
                return false;
            }

            if (token.IsRevoked)
            {
                return false;
            }

            token.RevokedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        // ==========================================================
        // CREATE REFRESH TOKEN
        // ==========================================================

        private RefreshToken CreateRefreshToken(
            User user)
        {
            const int refreshTokenDays = 7;

            return new RefreshToken
            {
                Token =
                    _jwtService.GenerateRefreshToken(),

                UserId =
                    user.User_id,

                CreatedAt =
                    DateTime.UtcNow,

                ExpiresAt =
                    DateTime.UtcNow.AddDays(
                        refreshTokenDays)
            };
        }
    }
}

