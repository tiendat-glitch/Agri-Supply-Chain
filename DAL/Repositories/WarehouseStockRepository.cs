using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class WarehouseStockRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public WarehouseStockRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<WarehouseStock> GetByWarehouse(int warehouseId)
        {
            var list = new List<WarehouseStock>();
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, warehouse_id, batch_id, quantity, unit, last_updated
                FROM dbo.warehouse_stock
                WHERE warehouse_id = @WarehouseId";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public WarehouseStock? GetOne(int warehouseId, int batchId)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, warehouse_id, batch_id, quantity, unit, last_updated
                FROM dbo.warehouse_stock
                WHERE warehouse_id = @WarehouseId AND batch_id = @BatchId";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
            cmd.Parameters.AddWithValue("@BatchId", batchId);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public WarehouseStock Upsert(int warehouseId, int batchId, decimal deltaQty, string? unit)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            using var tran = conn.BeginTransaction();

            try
            {
                const string selectSql = @"
                    SELECT id, warehouse_id, batch_id, quantity, unit, last_updated
                    FROM dbo.warehouse_stock
                    WHERE warehouse_id = @WarehouseId AND batch_id = @BatchId
                    FOR UPDATE";

                WarehouseStock? existing = null;
                using (var selectCmd = new SqlCommand(selectSql, conn, tran))
                {
                    selectCmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    selectCmd.Parameters.AddWithValue("@BatchId", batchId);
                    using var reader = selectCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        existing = Map(reader);
                    }
                }

                if (existing == null)
                {
                    const string insertSql = @"
                        INSERT INTO dbo.warehouse_stock
                            (warehouse_id, batch_id, quantity, unit, last_updated)
                        VALUES
                            (@WarehouseId, @BatchId, @Quantity, @Unit, SYSUTCDATETIME());
                        SELECT SCOPE_IDENTITY();";
                    using var insertCmd = new SqlCommand(insertSql, conn, tran);
                    insertCmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                    insertCmd.Parameters.AddWithValue("@BatchId", batchId);
                    insertCmd.Parameters.AddWithValue("@Quantity", deltaQty);
                    insertCmd.Parameters.AddWithValue("@Unit", (object?)unit ?? "kg");
                    var newId = Convert.ToInt32(insertCmd.ExecuteScalar());
                    tran.Commit();
                    return new WarehouseStock
                    {
                        Id = newId,
                        WarehouseId = warehouseId,
                        BatchId = batchId,
                        Quantity = deltaQty,
                        Unit = unit ?? "kg",
                        LastUpdated = DateTime.UtcNow
                    };
                }
                else
                {
                    var newQty = existing.Quantity + deltaQty;
                    const string updateSql = @"
                        UPDATE dbo.warehouse_stock
                        SET quantity = @Quantity,
                            unit = @Unit,
                            last_updated = SYSUTCDATETIME()
                        WHERE id = @Id";
                    using var updateCmd = new SqlCommand(updateSql, conn, tran);
                    updateCmd.Parameters.AddWithValue("@Id", existing.Id);
                    updateCmd.Parameters.AddWithValue("@Quantity", newQty);
                    updateCmd.Parameters.AddWithValue("@Unit", (object?)unit ?? (object?)existing.Unit ?? "kg");
                    updateCmd.ExecuteNonQuery();
                    tran.Commit();
                    existing.Quantity = newQty;
                    existing.Unit = unit ?? existing.Unit;
                    existing.LastUpdated = DateTime.UtcNow;
                    return existing;
                }
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        private static WarehouseStock Map(SqlDataReader reader)
        {
            return new WarehouseStock
            {
                Id = Convert.ToInt32(reader["id"]),
                WarehouseId = Convert.ToInt32(reader["warehouse_id"]),
                BatchId = Convert.ToInt32(reader["batch_id"]),
                Quantity = Convert.ToDecimal(reader["quantity"]),
                Unit = reader["unit"] == DBNull.Value ? null : reader["unit"].ToString(),
                LastUpdated = Convert.ToDateTime(reader["last_updated"])
            };
        }
    }
}


