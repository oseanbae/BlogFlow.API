using BlogFlow.API.Data;
using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using BlogFlow.API.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogFlow.API.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly AppDbContext _context; // delete later
        private readonly JwtSettings _jwt;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public AuthServices(
            AppDbContext context,
            IRefreshTokenRepository refrshTokenRepo,
            IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _refreshTokenRepo = refrshTokenRepo;
            _jwt = jwtOptions.Value;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

            if (existingUser != null)
                throw new Exception("Username or Email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash
            };
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var accessToken = GenerateToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiry = refreshToken.ExpiresAt
            };
        }
        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid Credentials");

            await RemoveExpiredRefreshTokensAsync(user.Id);

            var accessToken = GenerateToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiry = refreshToken.ExpiresAt
            };
        }
        public async Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO request)
        {
            var existingToken = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            // Check revocation before expiry — a revoked token could indicate theft.
            // If someone is reusing a revoked token, wipe all sessions.
            if (existingToken.IsRevoked)
            {
                await RevokeAllUserTokensAsync(existingToken.UserId, "Reuse of revoked token detected");
                throw new UnauthorizedAccessException("Token reuse detected. All sessions revoked.");
            }
            if (existingToken.IsExpired)
                throw new UnauthorizedAccessException("Expired refresh token.");

            // rotate
            var newRefreshToken = await GenerateRefreshTokenAsync(existingToken.UserId);

            //revoking the token
            await _refreshTokenRepo.RevokeAsync(existingToken, "Rotated", newRefreshToken.Token);

            var accessToken = GenerateToken(existingToken.User);

            return new AuthResponseDTO
            {
                Id = existingToken.User.Id,
                Username = existingToken.User.Username,
                Email = existingToken.User.Email,
                Role = existingToken.User.Role,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiry = newRefreshToken.ExpiresAt
            };
        }
        public async Task RevokeAsync(RevokeRequestDTO request, Guid userId)
        {
            var token = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken);

            if (token == null || !token.IsActive)
                throw new UnauthorizedAccessException("Invalid token");

            await _refreshTokenRepo.RevokeAsync(token, "Revoked by user", null);
        }
        private string GenerateToken(User user)
        {
            // Token lifetime configuration (in minutes)
            var expiryMinutes = _jwt.AccessTokenExpiryMinutes;

            // Create symmetric security key from secret string (used for HMAC signing)
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));

            // Define signing algorithm and credentials (HMAC SHA-256)
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Convert user role enum/value into string for JWT claim storage
            var userRole = user.Role.ToString();

            // Define claims (data embedded inside the JWT payload)
            var claims = new[]
            {
                // "sub" (subject) usually represents the user ID
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

                // Standard email claim
                new Claim(JwtRegisteredClaimNames.Email, user.Email),

                // Username stored as Name claim (used by identity frameworks)
                new Claim(ClaimTypes.Name, user.Username),

                // Role claim used for authorization [Authorize(Roles = "...")]
                new Claim(ClaimTypes.Role, userRole),

                // Unique token identifier (prevents replay / helps revoke tracking)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Build token descriptor (this defines structure + metadata of JWT)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Attach identity + claims payload
                Subject = new ClaimsIdentity(claims),

                // Expiration time (important for security)
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),

                // Issuer validation (server that created the token)
                Issuer = _jwt.Issuer,

                // Audience validation (who this token is intended for)
                Audience = _jwt.Audience,

                // Signing credentials ensure token integrity and authenticity
                SigningCredentials = credentials
            };

            // Handler responsible for creating and writing JWT tokens
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create token object based on descriptor
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize token into compact JWT string (header.payload.signature)
            return tokenHandler.WriteToken(securityToken);
        }
        private async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId)
        {
            // Generate secure random refresh token
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays)
            };

            return await _refreshTokenRepo.CreateAsync(refreshToken);

        }
        private async Task RemoveExpiredRefreshTokensAsync(Guid userId)
        {
            await _refreshTokenRepo.RemoveExpiredAsync(userId);
        }
        private async Task RevokeAllUserTokensAsync(Guid userId, string reason)
        {
            await _refreshTokenRepo.RevokeAllUserTokensAsync(userId, reason);
        }
    }
}
