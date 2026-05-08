
using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using BlogFlow.API.Settings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogFlow.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwt;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IUserRepository _userRepo;
        public AuthService(
            IRefreshTokenRepository refrshTokenRepo,
            IUserRepository userRepo,
            IOptions<JwtSettings> jwtOptions)
        {
            _refreshTokenRepo = refrshTokenRepo;
            _userRepo = userRepo;
            _jwt = jwtOptions.Value;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var username = request.Username.ToLowerInvariant();
            var email = request.Email.ToLowerInvariant();

            var existingUserByUsername = await _userRepo.GetByUsernameAsync(username);
            if (existingUserByUsername != null)
                throw new InvalidOperationException("Username already exists.");

            var existingUserByEmail = await _userRepo.GetByEmailAsync(email);
            if (existingUserByEmail != null)
                throw new InvalidOperationException("Email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash
            };

            await _userRepo.CreateAsync(user);

            var accessToken = GenerateToken(user);

            var (rawToken, storedToken) = await GenerateRefreshTokenAsync(user.Id);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = rawToken,
                RefreshTokenExpiry = storedToken.ExpiresAt
            };
        }
        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var identifier = request.UsernameOrEmail.ToLowerInvariant();
            var user = await _userRepo.GetByUsernameOrEmailAsync(identifier);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid Credentials");

            await _refreshTokenRepo.RemoveExpiredAsync(user.Id);

            var accessToken = GenerateToken(user);

            var (rawToken, storedToken) = await GenerateRefreshTokenAsync(user.Id);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = rawToken,
                RefreshTokenExpiry = storedToken.ExpiresAt
            };
        }
        public async Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO request)
        {
            var hashed = HashToken(request.RefreshToken);
            var existingToken = await _refreshTokenRepo.GetByHashedTokenAsync(hashed)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            // Check revocation before expiry — a revoked token could indicate theft.
            // If someone is reusing a revoked token, wipe all sessions.
            if (existingToken.IsRevoked)
            {
                await _refreshTokenRepo.RevokeAllUserTokensAsync(existingToken.UserId, "Reuse of revoked token detected");
                throw new UnauthorizedAccessException("Token reuse detected. All sessions revoked.");
            }
            if (existingToken.IsExpired)
                throw new UnauthorizedAccessException("Expired refresh token.");


            // get user BEFORE generating new token
            var user = existingToken.User;

            // rotate refresh token
            var (rawToken, refreshTokenEntity) = await GenerateRefreshTokenAsync(existingToken.UserId);

            // revoke old token
            await _refreshTokenRepo.RevokeAsync(
                existingToken,
                "Rotated",
                HashToken(rawToken));

            var accessToken = GenerateToken(user);

            return new AuthResponseDTO
            {
                Id = existingToken.User.Id,
                Username = existingToken.User.Username,
                Email = existingToken.User.Email,
                Role = existingToken.User.Role,
                AccessToken = accessToken,
                RefreshToken = rawToken,
                RefreshTokenExpiry = refreshTokenEntity.ExpiresAt
            };
        }
        public async Task RevokeAsync(RevokeRequestDTO request, UserContext user)
        {
            var hashed = HashToken(request.RefreshToken);

            var token = await _refreshTokenRepo.GetByHashedTokenAsync(hashed) 
                ?? throw new UnauthorizedAccessException("Invalid token");

            if (!token.IsActive)
                throw new UnauthorizedAccessException("Token expired or revoked");

            if (token.UserId != user.UserId)
                throw new UnauthorizedAccessException("Invalid token ownership");

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
                // NameIdentifier claim is used by ASP.NET for user identity mapping
                // (used by User.FindFirst(ClaimTypes.NameIdentifier))
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

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
        private async Task<(string RawToken, RefreshToken StoredToken)> GenerateRefreshTokenAsync(Guid userId)
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshToken = new RefreshToken
            {
                Token = HashToken(rawToken),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays)
            };

            await _refreshTokenRepo.CreateAsync(refreshToken);

            return (rawToken, refreshToken);
        }
        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
