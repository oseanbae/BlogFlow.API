using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlogFlow.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUser;

        public AuthController(IAuthService authServices, ICurrentUserService currentUser)
        {
            _authService = authServices;
            _currentUser = currentUser;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO dto, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(dto, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginAsync(LoginRequestDTO dto, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(result);
        }

        [EnableRateLimiting("refresh")]
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> RefreshAsync([FromBody] RefreshTokenRequestDTO dto, CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshAsync(dto, cancellationToken);
            return Ok(result);
        }

        [EnableRateLimiting("revoke")]
        [HttpPost("revoke")]
        [Authorize]
        public async Task<ActionResult> RevokeAsync([FromBody] RevokeRequestDTO request, CancellationToken cancellationToken)
        {
            await _authService.RevokeAsync(request, _currentUser.GetRequiredUserId(), cancellationToken);
            return NoContent();
        }
    }
}