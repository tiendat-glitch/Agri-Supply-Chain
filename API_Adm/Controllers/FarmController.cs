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
    [Route("api/farm")]
    public class FarmController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public FarmController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'");
        }

        [HttpGet]
        public async Task<IActionResult> GetFarms()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var sql = @"
            SELECT id, name, owner_name, location, contact_info, certifications, created_at
            FROM [farms] ";
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                var farms = new List<object>();
                while (await reader.ReadAsync())
                {
                    farms.Add(new
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        OwnerName = reader.IsDBNull("owner_name") ? null : reader.GetString("owner_name"),
                        Location = reader.IsDBNull("location") ? null : reader.GetString("location"),
                        ContactInfo = reader.IsDBNull("contact_info") ? null : reader.GetString("contact_info"),
                        Certifications = reader.IsDBNull("certifications") ? null : reader.GetString("certifications"),
                        CreatedAt = reader.GetDateTime("created_at")
                    });
                }
                return Ok(new { success = true, data = farms });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi, vui lòng thử lại sau", error = ex.Message });

            }
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFarmsByID(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
            SELECT id, name, owner_name, location, contact_info, certifications, created_at
            FROM farms
            WHERE id = @id";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy farm"
                    });
                }

                var farm = new
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    OwnerName = reader.IsDBNull("owner_name") ? null : reader.GetString("owner_name"),
                    Location = reader.IsDBNull("location") ? null : reader.GetString("location"),
                    ContactInfo = reader.IsDBNull("contact_info") ? null : reader.GetString("contact_info"),
                    Certifications = reader.IsDBNull("certifications") ? null : reader.GetString("certifications"),
                    CreatedAt = reader.GetDateTime("created_at")

                };

                return Ok(new { success = true, data = farm });
            }
            catch
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi, vui lòng thử lại sau"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateFarm([FromBody] CreateFarmRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var sql = @"
            INSERT INTO farms (name, owner_name, location, contact_info, certifications, created_at)
            VALUES (@Name, @OwnerName, @Location, @ContactInfo, @Certifications, @CreatedAt);
            SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", request.Name);
                command.Parameters.AddWithValue("@OwnerName", (object?)request.OwnerName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Location", (object?)request.Location ?? DBNull.Value);
                command.Parameters.AddWithValue("@ContactInfo", (object?)request.ContactInfo ?? DBNull.Value);
                command.Parameters.AddWithValue("@Certifications", (object?)request.Certifications ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                var result = await command.ExecuteScalarAsync();
                var newFarmId = Convert.ToInt32(result);
                return Ok(new
                {
                    success = true,
                    message = "Tạo farm thành công",
                    farmId = newFarmId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi, vui lòng thử lại sau",
                    error = ex.Message
                });
            }
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateFarm(int id, [FromBody] UpdateFarmRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var sql = @"
            UPDATE farms
            SET name = @Name,
                owner_name = @OwnerName,
                location = @Location,
                contact_info = @ContactInfo,
                certifications = @Certifications
            WHERE id = @id";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", request.Name);
                command.Parameters.AddWithValue("@OwnerName", (object?)request.OwnerName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Location", (object?)request.Location ?? DBNull.Value);
                command.Parameters.AddWithValue("@ContactInfo", (object?)request.ContactInfo ?? DBNull.Value);
                command.Parameters.AddWithValue("@Certifications", (object?)request.Certifications ?? DBNull.Value);
                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy farm để cập nhật"
                    });
                }
                return Ok(new
                {
                    success = true,
                    message = "Cập nhật farm thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi, vui lòng thử lại sau",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var sql = @"
            DELETE FROM farms
            WHERE id = @id";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy farm để xóa"
                    });
                }
                return Ok(new
                {
                    success = true,
                    message = "Xóa farm thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi, vui lòng thử lại sau",
                    error = ex.Message
                });
            }
        }
    }
}
