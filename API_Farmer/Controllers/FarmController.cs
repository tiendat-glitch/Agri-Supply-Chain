using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using FarmDTOs.DTO;

namespace API_Farmer.Controllers
{
    [ApiController]
    [Route("api/farmer/farm")]
    [Authorize(Roles = "farmer")]
    public class FarmController : ControllerBase
    {
        private readonly string _connectionString;

        public FarmController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = new List<FarmDTO>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Farms_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapFarm(reader));
            }

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Farms_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return NotFound("Không tìm thấy farm");

            return Ok(MapFarm(reader));
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFarmDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Farms_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Name", dto.Name);
            cmd.Parameters.AddWithValue("@OwnerName", (object?)dto.OwnerName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Location", (object?)dto.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactInfo", (object?)dto.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Certifications", (object?)dto.Certifications ?? DBNull.Value);

            var newId = await cmd.ExecuteScalarAsync();

            return Ok(new
            {
                success = true,
                message = "Tạo farm thành công",
                farmId = Convert.ToInt32(newId)
            });
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFarmDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Farms_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", dto.Name);
            cmd.Parameters.AddWithValue("@OwnerName", (object?)dto.OwnerName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Location", (object?)dto.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactInfo", (object?)dto.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Certifications", (object?)dto.Certifications ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            if (Convert.ToInt32(result) == 0)
                return NotFound("Không tìm thấy farm để cập nhật");

            return Ok(new { success = true, message = "Cập nhật farm thành công" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Farms_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            var result = await cmd.ExecuteScalarAsync();
            if (Convert.ToInt32(result) == 0)
                return NotFound("Không tìm thấy farm để xóa");

            return Ok(new { success = true, message = "Xóa farm thành công" });
        }

        private static FarmDTO MapFarm(SqlDataReader reader)
        {
            return new FarmDTO
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                OwnerName = reader.IsDBNull("owner_name") ? null : reader.GetString("owner_name"),
                Location = reader.IsDBNull("location") ? null : reader.GetString("location"),
                ContactInfo = reader.IsDBNull("contact_info") ? null : reader.GetString("contact_info"),
                Certifications = reader.IsDBNull("certifications") ? null : reader.GetString("certifications"),
                CreatedAt = reader.GetDateTime("created_at")
            };
        }
    }
}
