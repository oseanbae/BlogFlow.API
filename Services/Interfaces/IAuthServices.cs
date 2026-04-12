using BlogFlow.API.DTOs.Auth;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO request);
        Task RevokeAsync(RevokeRequestDTO request, Guid userId);
    }
}
