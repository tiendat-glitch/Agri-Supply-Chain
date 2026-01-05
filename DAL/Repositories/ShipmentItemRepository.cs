using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class ShipmentItemRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public ShipmentItemRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<ShipmentItem> GetByShipment(int shipmentId)
        {
            var list = new List<ShipmentItem>();
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, shipment_id, batch_id, quantity, unit
                FROM dbo.shipment_items
                WHERE shipment_id = @ShipmentId";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ShipmentId", shipmentId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public List<ShipmentItem> GetByShipmentIds(List<int> shipmentIds)
        {
            var list = new List<ShipmentItem>();
            if (shipmentIds == null || shipmentIds.Count == 0) return list;

            using SqlConnection conn = _dbHelper.GetConnection();
            // Build dynamic IN clause safely using table-valued parameter approach; for simplicity, use OR when small list.
            var inParams = string.Join(",", shipmentIds.Select((_, idx) => $"@p{idx}"));
            var sql = $@"
                SELECT id, shipment_id, batch_id, quantity, unit
                FROM dbo.shipment_items
                WHERE shipment_id IN ({inParams})";
            using var cmd = new SqlCommand(sql, conn);
            for (int i = 0; i < shipmentIds.Count; i++)
            {
                cmd.Parameters.AddWithValue($"@p{i}", shipmentIds[i]);
            }
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public int Create(ShipmentItem entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                INSERT INTO dbo.shipment_items
                    (shipment_id, batch_id, quantity, unit)
                VALUES
                    (@ShipmentId, @BatchId, @Quantity, @Unit);
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ShipmentId", entity.ShipmentId);
            cmd.Parameters.AddWithValue("@BatchId", entity.BatchId);
            cmd.Parameters.AddWithValue("@Quantity", entity.Quantity);
            cmd.Parameters.AddWithValue("@Unit", (object?)entity.Unit ?? DBNull.Value);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public bool Update(int id, ShipmentItem entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                UPDATE dbo.shipment_items
                SET batch_id = @BatchId,
                    quantity = @Quantity,
                    unit = @Unit
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@BatchId", entity.BatchId);
            cmd.Parameters.AddWithValue("@Quantity", entity.Quantity);
            cmd.Parameters.AddWithValue("@Unit", (object?)entity.Unit ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = "DELETE FROM dbo.shipment_items WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static ShipmentItem Map(SqlDataReader reader)
        {
            return new ShipmentItem
            {
                Id = Convert.ToInt32(reader["id"]),
                ShipmentId = Convert.ToInt32(reader["shipment_id"]),
                BatchId = Convert.ToInt32(reader["batch_id"]),
                Quantity = Convert.ToDecimal(reader["quantity"]),
                Unit = reader["unit"] == DBNull.Value ? null : reader["unit"].ToString()
            };
        }
    }
}

