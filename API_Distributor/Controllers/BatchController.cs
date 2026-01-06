using BLL;
using BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_Adm.Controllers
{
    [ApiController]
    [Route("api/distributor/batch")]
    [Authorize(Roles = "distributor")]
    public class BatchController : ControllerBase
    {
        private readonly string _connectionString;

        public BatchController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'");
        }

        // GET all batches
        [HttpGet]
        public async Task<IActionResult> GetAllBatches()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT id, batch_code, product_id, farm_id, created_by_user_id, harvest_date, quantity, unit, expiry_date, status, created_at
                        FROM batches";
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            var batches = new List<object>();
            while (await reader.ReadAsync())
            {
                batches.Add(new
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    BatchCode = reader.GetString(reader.GetOrdinal("batch_code")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                    FarmId = reader.GetInt32(reader.GetOrdinal("farm_id")),
                    CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("created_by_user_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("created_by_user_id")),
                    HarvestDate = reader.IsDBNull(reader.GetOrdinal("harvest_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("harvest_date")),
                    Quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("quantity")),
                    Unit = reader.IsDBNull(reader.GetOrdinal("unit")) ? null : reader.GetString(reader.GetOrdinal("unit")),
                    ExpiryDate = reader.IsDBNull(reader.GetOrdinal("expiry_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("expiry_date")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }

            return Ok(new { success = true, data = batches });
        }

        // GET batch by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBatchById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT id, batch_code, product_id, farm_id, created_by_user_id, harvest_date, quantity, unit, expiry_date, status, created_at
                        FROM batches WHERE id = @id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return NotFound(new { success = false, message = "Batch not found" });

            var batch = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                BatchCode = reader.GetString(reader.GetOrdinal("batch_code")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                FarmId = reader.GetInt32(reader.GetOrdinal("farm_id")),
                CreatedByUserId = reader.IsDBNull(reader.GetOrdinal("created_by_user_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("created_by_user_id")),
                HarvestDate = reader.IsDBNull(reader.GetOrdinal("harvest_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("harvest_date")),
                Quantity = reader.IsDBNull(reader.GetOrdinal("quantity")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("quantity")),
                Unit = reader.IsDBNull(reader.GetOrdinal("unit")) ? null : reader.GetString(reader.GetOrdinal("unit")),
                ExpiryDate = reader.IsDBNull(reader.GetOrdinal("expiry_date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("expiry_date")),
                Status = reader.GetString(reader.GetOrdinal("status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };

            return Ok(new { success = true, data = batch });
        }

        
    }
}
