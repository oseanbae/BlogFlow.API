using BlogFlow.API.DTOs.User;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ICurrentUserService _currentUser;

        public UserController(IUserService service, ICurrentUserService currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserPublicProfileDTO>> GetProfileAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetPublicProfileAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserProfileDTO>> GetMyProfileAsync(
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.GetRequiredUserId();
            var result = await _service.GetMyProfileAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(UserUpdateDTO dto, CancellationToken cancellationToken)
        {
            await _service.UpdateProfileAsync(dto, _currentUser.GetRequiredUserId(), cancellationToken);
            return NoContent();
        }

        [HttpPatch("me/password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] UserChangePasswordDTO dto, CancellationToken cancellationToken)
        {
            await _service.ChangePasswordAsync(_currentUser.GetRequiredUserId(), dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("me")]
        [Authorize]
        public async Task<ActionResult> DeleteAccount(CancellationToken cancellationToken)
        {
            await _service.DeleteOwnAccountAsync(_currentUser.GetRequiredUserId(), cancellationToken);
            return NoContent();
        }
    }
}
