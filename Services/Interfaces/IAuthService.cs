using BlogFlow.API.DTOs.Auth;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken);
        Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken);
        Task RevokeAsync(RevokeRequestDTO request, Guid userId, CancellationToken cancellationToken);
    }
}
