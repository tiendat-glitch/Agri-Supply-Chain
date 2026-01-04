using API_Adm.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_Adm.Controllers
{
    [ApiController]
    [Route("api/admin/batch")]
    
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

        // POST create batch
        [HttpPost]
        public async Task<IActionResult> CreateBatch([FromBody] CreateBatchRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO batches
                (batch_code, product_id, farm_id, created_by_user_id, harvest_date, quantity, unit, expiry_date, status, created_at)
                VALUES
                (@BatchCode, @ProductId, @FarmId, @CreatedByUserId, @HarvestDate, @Quantity, @Unit, @ExpiryDate, @Status, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BatchCode", request.BatchCode);
            command.Parameters.AddWithValue("@ProductId", request.ProductId);
            command.Parameters.AddWithValue("@FarmId", request.FarmId);
            command.Parameters.AddWithValue("@CreatedByUserId", (object?)request.CreatedByUserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@HarvestDate", (object?)request.HarvestDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Quantity", (object?)request.Quantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@Unit", (object?)request.Unit ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", (object?)request.ExpiryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)request.Status ?? "pending");

            var batchId = Convert.ToInt32(await command.ExecuteScalarAsync());

            return Ok(new { success = true, message = "Batch created", batchId });
        }

        // PUT update batch
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBatch(int id, [FromBody] UpdateBatchRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                UPDATE batches
                SET batch_code = COALESCE(@BatchCode, batch_code),
                    product_id = COALESCE(@ProductId, product_id),
                    farm_id = COALESCE(@FarmId, farm_id),
                    created_by_user_id = COALESCE(@CreatedByUserId, created_by_user_id),
                    harvest_date = COALESCE(@HarvestDate, harvest_date),
                    quantity = COALESCE(@Quantity, quantity),
                    unit = COALESCE(@Unit, unit),
                    expiry_date = COALESCE(@ExpiryDate, expiry_date),
                    status = COALESCE(@Status, status)
                WHERE id = @id";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BatchCode", (object?)request.BatchCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductId", (object?)request.ProductId ?? DBNull.Value);
            command.Parameters.AddWithValue("@FarmId", (object?)request.FarmId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedByUserId", (object?)request.CreatedByUserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@HarvestDate", (object?)request.HarvestDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Quantity", (object?)request.Quantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@Unit", (object?)request.Unit ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExpiryDate", (object?)request.ExpiryDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)request.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound(new { success = false, message = "Batch not found" });

            return Ok(new { success = true, message = "Batch updated" });
        }

        // DELETE batch
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"DELETE FROM batches WHERE id = @id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound(new { success = false, message = "Batch not found" });

            return Ok(new { success = true, message = "Batch deleted" });
        }
    }
}
