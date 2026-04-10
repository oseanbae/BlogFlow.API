using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authservices)
        {
            _authServices = authservices;
        }

        //POST: /api/v1/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO dto)
        {
            var result = await _authServices.RegisterAsync(dto);
            return Ok(result);
        }

        //POST: /api/v1/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginAsync(LoginRequestDTO dto)
        {
            var result = await _authServices.LoginAsync(dto);
            return Ok(result);
        }

    }
}
