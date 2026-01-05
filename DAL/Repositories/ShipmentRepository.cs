using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class ShipmentRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public ShipmentRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Shipment> GetAll()
        {
            var list = new List<Shipment>();
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, shipment_code, planned_by_user_id, distributor_id,
                       from_type, from_id, to_type, to_id,
                       vehicle_info, driver_info,
                       departure_time, arrival_time, status, created_at, notes
                FROM dbo.shipments";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public Shipment? GetById(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, shipment_code, planned_by_user_id, distributor_id,
                       from_type, from_id, to_type, to_id,
                       vehicle_info, driver_info,
                       departure_time, arrival_time, status, created_at, notes
                FROM dbo.shipments
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public int Create(Shipment entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                INSERT INTO dbo.shipments
                    (shipment_code, planned_by_user_id, distributor_id,
                     from_type, from_id, to_type, to_id,
                     vehicle_info, driver_info,
                     departure_time, arrival_time, status, created_at, notes)
                VALUES
                    (@ShipmentCode, @PlannedByUserId, @DistributorId,
                     @FromType, @FromId, @ToType, @ToId,
                     @VehicleInfo, @DriverInfo,
                     @DepartureTime, @ArrivalTime, @Status, SYSUTCDATETIME(), @Notes);
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ShipmentCode", entity.ShipmentCode);
            cmd.Parameters.AddWithValue("@PlannedByUserId", (object?)entity.PlannedByUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DistributorId", (object?)entity.DistributorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FromType", entity.FromType);
            cmd.Parameters.AddWithValue("@FromId", (object?)entity.FromId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToType", entity.ToType);
            cmd.Parameters.AddWithValue("@ToId", (object?)entity.ToId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VehicleInfo", (object?)entity.VehicleInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DriverInfo", (object?)entity.DriverInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartureTime", (object?)entity.DepartureTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ArrivalTime", (object?)entity.ArrivalTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", entity.Status);
            cmd.Parameters.AddWithValue("@Notes", (object?)entity.Notes ?? DBNull.Value);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public bool Update(int id, Shipment entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                UPDATE dbo.shipments
                SET distributor_id = @DistributorId,
                    vehicle_info = @VehicleInfo,
                    driver_info = @DriverInfo,
                    departure_time = @DepartureTime,
                    arrival_time = @ArrivalTime,
                    notes = @Notes
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@DistributorId", (object?)entity.DistributorId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@VehicleInfo", (object?)entity.VehicleInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DriverInfo", (object?)entity.DriverInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartureTime", (object?)entity.DepartureTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ArrivalTime", (object?)entity.ArrivalTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object?)entity.Notes ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = "DELETE FROM dbo.shipments WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateStatus(int id, string status, DateTime? arrivalTime)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                UPDATE dbo.shipments
                SET status = @Status,
                    arrival_time = @ArrivalTime
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@ArrivalTime", (object?)arrivalTime ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Shipment Map(SqlDataReader reader)
        {
            return new Shipment
            {
                Id = Convert.ToInt32(reader["id"]),
                ShipmentCode = reader["shipment_code"].ToString()!,
                PlannedByUserId = reader["planned_by_user_id"] == DBNull.Value ? null : Convert.ToInt32(reader["planned_by_user_id"]),
                DistributorId = reader["distributor_id"] == DBNull.Value ? null : Convert.ToInt32(reader["distributor_id"]),
                FromType = reader["from_type"].ToString()!,
                FromId = reader["from_id"] == DBNull.Value ? null : Convert.ToInt32(reader["from_id"]),
                ToType = reader["to_type"].ToString()!,
                ToId = reader["to_id"] == DBNull.Value ? null : Convert.ToInt32(reader["to_id"]),
                VehicleInfo = reader["vehicle_info"] == DBNull.Value ? null : reader["vehicle_info"].ToString(),
                DriverInfo = reader["driver_info"] == DBNull.Value ? null : reader["driver_info"].ToString(),
                DepartureTime = reader["departure_time"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["departure_time"]),
                ArrivalTime = reader["arrival_time"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["arrival_time"]),
                Status = reader["status"].ToString()!,
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                Notes = reader["notes"] == DBNull.Value ? null : reader["notes"].ToString()
            };
        }
    }
}

