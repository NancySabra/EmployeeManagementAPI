using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Dapper;
using EmployeeManagement.Domain.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using EmployeeManagement.DAL.Interfaces;

namespace EmployeeManagement.DAL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;

        public AuthRepository(IDbConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            // Get user by username (SP)
            var user = await _db.QueryFirstOrDefaultAsync<dynamic>(
                "dbo.usp_User_GetByUsername",
                new { Username = dto.Username },
                commandType: CommandType.StoredProcedure);

            if (user == null) return null;

            string storedPassword = (string)user.Password;

            // Support BOTH cases:
            // 1) old plain text passwords in DB
            // 2) new BCrypt hashed passwords in DB
            bool passwordOk =
                storedPassword.StartsWith("$2") // BCrypt hashes start with $2a/$2b/$2y
                ? BCrypt.Net.BCrypt.Verify(dto.Password, storedPassword)
                : string.Equals(dto.Password, storedPassword);

            if (!passwordOk) return null;

            int userId = (int)user.UserId;
            string username = (string)user.Username;

            // Build JWT
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection["Key"]!;
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"]!);

            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                // optional role claim (you can add later if you implement roles)
                new Claim(ClaimTypes.Role, "User")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = userId,
                Username = username
            };
        }
    }
}