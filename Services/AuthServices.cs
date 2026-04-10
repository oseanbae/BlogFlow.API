using BlogFlow.API.Data;
using BlogFlow.API.DTOs.Auth;
using BlogFlow.API.Models;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogFlow.API.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthServices(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
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

            var token = GenerateToken(user);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                throw new Exception("Account doesn't exist.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Password Incorrect.");

            var token = GenerateToken(user); 

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        private string GenerateToken(User user)
        {
            // Load JWT configuration section from appsettings.json (or equivalent config source)
            var jwt = _config.GetSection("JwtSettings");

            // Retrieve the secret key used to sign the token.
            // If missing → fail fast because token generation is impossible without it.
            var key = jwt["Secret"] ?? throw new Exception("JWT Secret is missing");

            // These define who issued the token and who it is meant for (validation later)
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];

            // Token lifetime configuration (in minutes)
            var expiryMinutes = Convert.ToDouble(jwt["AccessTokenExpiryMinutes"]);

            // Create symmetric security key from secret string (used for HMAC signing)
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

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
                Issuer = issuer,

                // Audience validation (who this token is intended for)
                Audience = audience,

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
    }
}
