using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authservices)
        {
            _authServices = authservices;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO dto)
        {
            var result = await _authServices.RegisterAsync(dto);
            return Ok(result);
        }

        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginAsync(LoginRequestDTO dto)
        {
            var result = await _authServices.LoginAsync(dto);
            return Ok(result);
        }

        [EnableRateLimiting("refresh")]
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> RefreshAsync([FromBody] RefreshTokenRequestDTO dto)
        {
            var result = await _authServices.RefreshAsync(dto);
            return Ok(result);
        }

        [EnableRateLimiting("revoke")]
        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeAsync([FromBody] RevokeRequestDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user identity");

            await _authServices.RevokeAsync(request, userId);

            return NoContent();
        }
    }
}
