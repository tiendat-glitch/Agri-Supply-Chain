using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class WarehouseRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public WarehouseRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Warehouse> GetAll()
        {
            var list = new List<Warehouse>();
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, name, location, contact_info, is_active, created_at
                FROM dbo.warehouses";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public Warehouse? GetById(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, name, location, contact_info, is_active, created_at
                FROM dbo.warehouses
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public int Create(Warehouse entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                INSERT INTO dbo.warehouses
                    (name, location, contact_info, is_active, created_at)
                VALUES
                    (@Name, @Location, @ContactInfo, @IsActive, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", entity.Name);
            cmd.Parameters.AddWithValue("@Location", (object?)entity.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactInfo", (object?)entity.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public bool Update(int id, Warehouse entity)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                UPDATE dbo.warehouses
                SET name = @Name,
                    location = @Location,
                    contact_info = @ContactInfo,
                    is_active = @IsActive
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", entity.Name);
            cmd.Parameters.AddWithValue("@Location", (object?)entity.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactInfo", (object?)entity.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = "DELETE FROM dbo.warehouses WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Warehouse Map(SqlDataReader reader)
        {
            return new Warehouse
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"].ToString()!,
                Location = reader["location"] == DBNull.Value ? null : reader["location"].ToString(),
                ContactInfo = reader["contact_info"] == DBNull.Value ? null : reader["contact_info"].ToString(),
                IsActive = Convert.ToBoolean(reader["is_active"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
}


