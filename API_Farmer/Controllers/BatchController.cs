using BatchDTOs.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace API_Farmer.Controllers
{
    [ApiController]
    [Route("api/farmer/batch")]
    [Authorize(Roles = "farmer")]
    public class BatchesController : ControllerBase
    {
        private readonly string _connectionString;

        public BatchesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = new List<BatchDTO>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Batches_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapBatch(reader));
            }

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Batches_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return NotFound();

            return Ok(MapBatch(reader));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Batches_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@BatchCode", dto.BatchCode);
            cmd.Parameters.AddWithValue("@ProductId", dto.ProductId);
            cmd.Parameters.AddWithValue("@FarmId", dto.FarmId);
            cmd.Parameters.AddWithValue("@CreatedByUserId", (object?)GetCurrentUserId() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HarvestDate", (object?)dto.HarvestDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", (object?)dto.Quantity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", (object?)dto.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiryDate", (object?)dto.ExpiryDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", dto.Status);

            var newId = await cmd.ExecuteScalarAsync();

            return Ok(new
            {
                success = true,
                message = "Tạo batch thành công",
                batchId = Convert.ToInt32(newId)
            });
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Batches_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@BatchCode", dto.BatchCode);
            cmd.Parameters.AddWithValue("@ProductId", dto.ProductId);
            cmd.Parameters.AddWithValue("@FarmId", dto.FarmId);
            cmd.Parameters.AddWithValue("@CreatedByUserId", (object?)GetCurrentUserId() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HarvestDate", (object?)dto.HarvestDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", (object?)dto.Quantity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", (object?)dto.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiryDate", (object?)dto.ExpiryDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", dto.Status);

            var result = await cmd.ExecuteScalarAsync();
            if (Convert.ToInt32(result) == 0)
                return NotFound("Không tìm thấy batch để cập nhật");

            return Ok(new { success = true, message = "Cập nhật batch thành công" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_Batches_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            var result = await cmd.ExecuteScalarAsync();
            if (Convert.ToInt32(result) == 0)
                return NotFound();

            return Ok(new { success = true, message = "Xóa batch thành công" });
        }

        private static BatchDTO MapBatch(SqlDataReader reader)
        {
            return new BatchDTO
            {
                Id = reader.GetInt32("id"),
                BatchCode = reader.GetString("batch_code"),
                ProductId = reader.GetInt32("product_id"),
                FarmId = reader.GetInt32("farm_id"),
                CreatedByUserId = reader.IsDBNull("created_by_user_id") ? null : reader.GetInt32("created_by_user_id"),
                HarvestDate = reader.IsDBNull("harvest_date") ? null : reader.GetDateTime("harvest_date"),
                Quantity = reader.IsDBNull("quantity") ? null : reader.GetDecimal("quantity"),
                Unit = reader.IsDBNull("unit") ? null : reader.GetString("unit"),
                ExpiryDate = reader.IsDBNull("expiry_date") ? null : reader.GetDateTime("expiry_date"),
                Status = reader.GetString("status"),
                CreatedAt = reader.GetDateTime("created_at")
            };
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var uid) ? uid : null;
        }
    }
}
