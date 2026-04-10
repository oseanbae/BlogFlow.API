using BlogFlow.API.DTOs.Auth;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IAuthServices
    {
        public Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
        public Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
    }
}
