using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;

namespace QuantityMeasurementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var response = _authService.Register(registerDto);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var response = _authService.Login(loginDto);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO googleLoginDto)
        {
            try
            {
                var response = await _authService.GoogleLoginAsync(googleLoginDto);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
