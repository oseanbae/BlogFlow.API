using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using BlogFlow.API.Settings;
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

            await ValidateUserDoesNotExist(username, email);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepo.CreateAsync(user);
            await _userRepo.SaveChangesAsync();

            return await IssueAuthResponseAsync(user);
        }
        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await ResolveUserAsync(request.UsernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid Credentials", "INVALID_CREDENTIALS");

            await _refreshTokenRepo.RemoveExpiredAsync(user.Id);

            return await IssueAuthResponseAsync(user);
        }
        public async Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO request)
        {
            var hashed = HashToken(request.RefreshToken);

            var existingToken = await _refreshTokenRepo.GetByHashedTokenAsync(hashed)
                ?? throw new UnauthorizedException("Invalid refresh token.", "INVALID_REFRESH_TOKEN");

            // Check revocation before expiry — a revoked token could indicate theft.
            // If someone is reusing a revoked token, wipe all sessions.
            if (existingToken.IsRevoked)
            {
                await _refreshTokenRepo.RevokeAllUserTokensAsync(
                    existingToken.UserId,
                    "Reuse of revoked token detected");

                throw new UnauthorizedException("Token reuse detected. All sessions revoked.", "TOKEN_REUSE_DETECTED");
            }

            if (existingToken.IsExpired)
                throw new UnauthorizedException("Expired refresh token.", "REFRESH_TOKEN_EXPIRED");

            // get user BEFORE generating new token
            var user = existingToken.User;
            // rotate refresh token
            var (rawToken, refreshTokenEntity) = await GenerateRefreshTokenAsync(user.Id);

            // revoke old token
            await _refreshTokenRepo.RevokeAsync(
                existingToken,
                "Rotated",
                HashToken(rawToken));

            var accessToken = GenerateToken(user);

            await _refreshTokenRepo.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = rawToken,
                RefreshTokenExpiry = refreshTokenEntity.ExpiresAt
            };
        }
        public async Task RevokeAsync(RevokeRequestDTO request, Guid userId)
        {
            var hashed = HashToken(request.RefreshToken);

            var token = await _refreshTokenRepo.GetByHashedTokenAsync(hashed)
                ?? throw new UnauthorizedException("Invalid token.", "TOKEN_NOT_FOUND");

            if (!token.IsActive)
                throw new UnauthorizedException("Token already expired or revoked.", "TOKEN_INACTIVE");

            if (token.UserId != userId)
                throw new ForbiddenException("You do not have permission to revoke this token.", "NOT_TOKEN_OWNER");

            await _refreshTokenRepo.RevokeAsync(token, "Revoked by user", null);

            await _refreshTokenRepo.SaveChangesAsync();
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

        private async Task<User?> ResolveUserAsync(string input)
        {
            var value = input.ToLowerInvariant();

            if (value.Contains('@'))
                return await _userRepo.GetByEmailAsync(value);

            return await _userRepo.GetByUsernameAsync(value);
        }

        private async Task ValidateUserDoesNotExist(string username, string email)
        {
            if (await _userRepo.GetByUsernameAsync(username) != null)
                throw new ConflictException($"Username '{username}' already exists.", "USERNAME_ALREADY_EXISTS");

            if (await _userRepo.GetByEmailAsync(email) != null)
                throw new ConflictException($"Email '{email}' already exists.", "EMAIL_ALREADY_EXISTS");
        }

        private async Task<AuthResponseDTO> IssueAuthResponseAsync(User user)
        {
            var accessToken = GenerateToken(user);

            var (rawToken, storedToken) =
                await GenerateRefreshTokenAsync(user.Id);

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
    }
}
