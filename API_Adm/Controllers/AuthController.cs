using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_Adm.DTO;

namespace API_Adm.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
            SELECT id, username, password_hash, full_name, email, phone, role
            FROM [users]
            WHERE username = @Username
        ";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", request.UserName);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return Unauthorized(new { success = false, message = "Sai tài khoản hoặc mật khẩu" });

                var id = reader.GetInt32("id");
                var username = reader.GetString("username");
                var passwordHash = reader.GetString("password_hash");

                var fullName = reader.IsDBNull("full_name") ? null : reader.GetString("full_name");
                var email = reader.IsDBNull("email") ? null : reader.GetString("email");
                var phone = reader.IsDBNull("phone") ? null : reader.GetString("phone");
                var role = reader.GetString("role");



                var user = new
                {
                    Id = id,
                    Username = username,
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Role = role
                };

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập thành công",
                    user,
                    token,
                    expiresIn = DateTime.UtcNow.AddHours(24)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private string GenerateJwtToken(dynamic user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWTsetting:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTsetting:Issuer"],
                audience: _configuration["JWTsetting:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JWTsetting:ExpireMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
