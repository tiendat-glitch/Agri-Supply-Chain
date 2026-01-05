using DAL.Helper;
using Model;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
    public class BatchRepository
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly AuditHelper _auditHelper;

        public BatchRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
            _auditHelper = new AuditHelper(_dbHelper);
        }

        public List<Batch> GetAll()
        {
            var list = new List<Batch>();
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, batch_code, product_id, farm_id, created_by_user_id,
                       harvest_date, quantity, unit, expiry_date, status, created_at
                FROM dbo.batches";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public Batch? GetById(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, batch_code, product_id, farm_id, created_by_user_id,
                       harvest_date, quantity, unit, expiry_date, status, created_at
                FROM dbo.batches
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public int Create(Batch batch)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
        INSERT INTO dbo.batches
            (batch_code, product_id, farm_id, created_by_user_id,
             harvest_date, quantity, unit, expiry_date, status, created_at)
        VALUES
            (@BatchCode, @ProductId, @FarmId, @CreatedByUserId,
             @HarvestDate, @Quantity, @Unit, @ExpiryDate, @Status, SYSUTCDATETIME());
        SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BatchCode", batch.BatchCode);
            cmd.Parameters.AddWithValue("@ProductId", batch.ProductId);
            cmd.Parameters.AddWithValue("@FarmId", batch.FarmId);
            cmd.Parameters.AddWithValue("@CreatedByUserId", (object?)batch.CreatedByUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HarvestDate", batch.HarvestDate.HasValue ? batch.HarvestDate.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", (object?)batch.Quantity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", (object?)batch.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiryDate", batch.ExpiryDate.HasValue ? batch.ExpiryDate.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", batch.Status);

            var result = cmd.ExecuteScalar();
            int id = Convert.ToInt32(result);

            // --- Ghi AuditLog ---
            _auditHelper.InsertLog(
                batch.CreatedByUserId,
                "INSERT",
                "batches",
                id.ToString(),
                oldValue: null,
                newValue: batch
            );

            return id;
        }

        public bool Update(int id, Batch batch)
        {
            // Lấy bản cũ để lưu AuditLog
            var oldBatch = GetById(id);
            if (oldBatch == null) return false;

            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                UPDATE dbo.batches
                SET batch_code = @BatchCode,
                    product_id = @ProductId,
                    farm_id = @FarmId,
                    created_by_user_id = @CreatedByUserId,
                    harvest_date = @HarvestDate,
                    quantity = @Quantity,
                    unit = @Unit,
                    expiry_date = @ExpiryDate,
                    status = @Status
                WHERE id = @Id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@BatchCode", batch.BatchCode);
            cmd.Parameters.AddWithValue("@ProductId", batch.ProductId);
            cmd.Parameters.AddWithValue("@FarmId", batch.FarmId);
            cmd.Parameters.AddWithValue("@CreatedByUserId", (object?)batch.CreatedByUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HarvestDate", batch.HarvestDate.HasValue ? batch.HarvestDate.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", (object?)batch.Quantity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", (object?)batch.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiryDate", batch.ExpiryDate.HasValue ? batch.ExpiryDate.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", batch.Status);

            bool ok = cmd.ExecuteNonQuery() > 0;

            if (ok)
            {
                _auditHelper.InsertLog(
                    batch.CreatedByUserId,
                    "UPDATE",
                    "batches",
                    id.ToString(),
                    oldValue: oldBatch,
                    newValue: batch
                );
            }

            return ok;
        }


        public bool Delete(int id)
        {
            var oldBatch = GetById(id);
            if (oldBatch == null) return false;

            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = "DELETE FROM dbo.batches WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            bool ok = cmd.ExecuteNonQuery() > 0;

            if (ok)
            {
                _auditHelper.InsertLog(
                    oldBatch.CreatedByUserId,
                    "DELETE",
                    "batches",
                    id.ToString(),
                    oldValue: oldBatch,
                    newValue: null
                );
            }

            return ok;
        }


        public bool UpdateStatus(int id, string status)
        {
            var oldBatch = GetById(id);
            if (oldBatch == null) return false;

            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = "UPDATE dbo.batches SET status = @Status WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Status", status);

            bool ok = cmd.ExecuteNonQuery() > 0;

            if (ok)
            {
                var newBatch = oldBatch;
                newBatch.Status = status;

                _auditHelper.InsertLog(
                    oldBatch.CreatedByUserId,
                    "UPDATE_STATUS",
                    "batches",
                    id.ToString(),
                    oldValue: oldBatch,
                    newValue: newBatch
                );
            }

            return ok;
        }
        private static Batch Map(SqlDataReader reader)
        {
            return new Batch
            {
                Id = Convert.ToInt32(reader["id"]),
                BatchCode = reader["batch_code"].ToString()!,
                ProductId = Convert.ToInt32(reader["product_id"]),
                FarmId = Convert.ToInt32(reader["farm_id"]),
                CreatedByUserId = reader["created_by_user_id"] == DBNull.Value ? null : Convert.ToInt32(reader["created_by_user_id"]),
                HarvestDate = reader["harvest_date"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(reader["harvest_date"])),
                Quantity = reader["quantity"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["quantity"]),
                Unit = reader["unit"] == DBNull.Value ? null : reader["unit"].ToString(),
                ExpiryDate = reader["expiry_date"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(reader["expiry_date"])),
                Status = reader["status"].ToString()!,
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
}