using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AuthResponseDTO>> RegisterAsync([FromBody] RegisterRequestDTO dto)
        {
            try
            {
                var result = await _authServices.RegisterAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginAsync([FromBody] LoginRequestDTO dto)
        {
            try
            {
                var result = await _authServices.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
