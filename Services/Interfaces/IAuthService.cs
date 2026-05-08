using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO request);
        Task RevokeAsync(RevokeRequestDTO request, UserContext user);
    }
}
