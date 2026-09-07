using BlogApi.BLL.Dtos.Auth;
using BlogApi.BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;


        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }


        // ============================================
        // REGISTER
        // ============================================

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDto dto)
        {
            try
            {
                var result =
                    await _authService.RegisterAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // ============================================
        // LOGIN
        // ============================================

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            try
            {
                var result =
                    await _authService.LoginAsync(dto);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // ============================================
        // REFRESH TOKEN
        // ============================================

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            RefreshTokenRequestDto dto)
        {
            try
            {
                var result =
                    await _authService.RefreshTokenAsync(dto);


                if (result == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message =
                            "Invalid or expired refresh token."
                    });
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // ============================================
        // LOGOUT
        // ============================================

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            RefreshTokenRequestDto dto)
        {
            try
            {
                var result =
                    await _authService
                        .RevokeRefreshTokenAsync(
                            dto.RefreshToken);


                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message =
                            "Invalid or already revoked refresh token."
                    });
                }


                return Ok(new
                {
                    success = true,
                    message = "Logout successful."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}