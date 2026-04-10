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
            var jwt = _config.GetSection("JwtSettings");

            var key = jwt["Secret"] ?? throw new Exception("JWT Secret is missing");
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var expiryMinutes = Convert.ToDouble(jwt["AccessTokenExpiryMinutes"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userRole = user.Role.ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, userRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
