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
    [Route("api/[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WarehousesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
            => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        // ==============================
        // GET: api/warehouses
        // ==============================
        [HttpGet("get-all")]
        
        public IActionResult GetAll()
        {
            var list = new List<WarehouseDto>();

            using var conn = GetConnection();
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM dbo.warehouses", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new WarehouseDto
                {
                    Id = (int)reader["id"],
                    Name = reader["name"].ToString()!,
                    Location = reader["location"] as string,
                    ContactInfo = reader["contact_info"] as string,
                    IsActive = (bool)reader["is_active"],
                    CreatedAt = (DateTime)reader["created_at"]
                });
            }

            return Ok(list);
        }

        // ==============================
        // GET: api/warehouses/5
        // ==============================
        [HttpGet("find/{id}")]
        public IActionResult GetById(int id)
        {
            using var conn = GetConnection();
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT * FROM dbo.warehouses WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return NotFound();

            var warehouse = new WarehouseDto
            {
                Id = (int)reader["id"],
                Name = reader["name"].ToString()!,
                Location = reader["location"] as string,
                ContactInfo = reader["contact_info"] as string,
                IsActive = (bool)reader["is_active"],
                CreatedAt = (DateTime)reader["created_at"]
            };

            return Ok(warehouse);
        }

        // ==============================
        // POST: api/warehouses
        // ==============================
        [HttpPost("create")]
        public IActionResult Create(CreateWarehouseDto dto)
        {
            using var conn = GetConnection();
            conn.Open();

            var sql = @"
            INSERT INTO dbo.warehouses (name, location, contact_info, is_active)
            VALUES (@name, @location, @contact_info, @is_active);
            SELECT SCOPE_IDENTITY();
        ";

            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", dto.Name);
            cmd.Parameters.AddWithValue("@location", (object?)dto.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@contact_info", (object?)dto.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_active", dto.IsActive);

            var newId = Convert.ToInt32(cmd.ExecuteScalar());

            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        // ==============================
        // PUT: api/warehouses/5
        // ==============================
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, CreateWarehouseDto dto)
        {
            using var conn = GetConnection();
            conn.Open();

            var sql = @"
            UPDATE dbo.warehouses
            SET name = @name,
                location = @location,
                contact_info = @contact_info,
                is_active = @is_active
            WHERE id = @id
        ";

            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", dto.Name);
            cmd.Parameters.AddWithValue("@location", (object?)dto.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@contact_info", (object?)dto.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_active", dto.IsActive);

            var rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound();

            return NoContent();
        }

        // ==============================
        // DELETE: api/warehouses/5
        // ==============================
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            using var conn = GetConnection();
            conn.Open();

            var cmd = new SqlCommand(
                "DELETE FROM dbo.warehouses WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                return NotFound();

            return NoContent();
        }
    }
}
