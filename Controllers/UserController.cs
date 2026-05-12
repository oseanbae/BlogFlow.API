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

        [HttpGet("{userId}")] // GET    api/v1/users/{userId}
        [AllowAnonymous]
        public async Task<ActionResult<UserReadDTO>> GetProfileAsync(Guid userId)
        {
            var result = await _service.GetUserByIdAsync(userId);
            return Ok(result);
        }

        [HttpPut("me")] // PUT    api/v1/users/me 
        [Authorize]
        public async Task<ActionResult> UpdateProfile(UserUpdateDTO dto)
        {
            var user = _currentUser.GetCurrentUser();
            await _service.UpdateProfileAsync(dto, _currentUser.GetRequiredUserId());
            return NoContent();
        }

        [HttpPatch("me/password")]  // PATCH  api/v1/users/me/password
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] UserChangePasswordDTO dto)
        {
            var id = _currentUser.GetRequiredUserId();
            await _service.ChangePasswordAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("me")] // DELETE api/v1/users/me
        [Authorize]
        public async Task<ActionResult> DeleteAccount()
        {
            await _service.DeleteOwnAccountAsync(_currentUser.GetRequiredUserId());
            return NoContent();
        }
    }
}
