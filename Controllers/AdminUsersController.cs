using BlogFlow.API.Domain.QueryParams;
using BlogFlow.API.DTOs.Admin;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.User;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserManagementService _service;

        public AdminUsersController(IUserManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResultDTO<AdminUserReadDTO>>> GetUsers([FromQuery] UserQueryParams query, CancellationToken cancellationToken)
        {
            var result = await _service.GetUsersAsync(query, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{userId}/role")]
        public async Task<IActionResult> ChangeRole(Guid userId, [FromBody] UserUpdateRoleDTO dto, CancellationToken cancellationToken)
        {
            await _service.ChangeRoleAsync(userId, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> SoftDeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            await _service.SoftDeleteUserAsync(userId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{userId}/restore")]
        public async Task<IActionResult> RestoreUser(Guid userId, CancellationToken cancellationToken)
        {
            await _service.RestoreUserAsync(userId, cancellationToken);
            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminStatsDTO>> GetStats(CancellationToken cancellationToken)
        {
            var stats = await _service.GetStatisticsAsync(cancellationToken);
            return Ok(stats);
        }
    }
}