using DAL.Helper;
using Microsoft.Data.SqlClient;
using Model;

namespace DAL.Repositories
{
    public class FarmRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public FarmRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Farm> GetAll()
        {
            var list = new List<Farm>();
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, name, owner_name, location, contact_info, certifications, created_at
                FROM dbo.farms";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public Farm? GetById(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                SELECT id, name, owner_name, location, contact_info, certifications, created_at
                FROM dbo.farms
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public int Create(Farm farm)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                INSERT INTO dbo.farms
                    (name, owner_name, location, contact_info, certifications, created_at)
                VALUES
                    (@Name, @OwnerName, @Location, @ContactInfo, @Certifications, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", farm.Name);
            cmd.Parameters.AddWithValue("@OwnerName", (object?)farm.OwnerName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Location", (object?)farm.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactInfo", (object?)farm.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Certifications", (object?)farm.Certifications ?? DBNull.Value);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public bool Update(int id, Farm farm)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = @"
                UPDATE dbo.farms
                SET name = @Name,
                    owner_name = @OwnerName,
                    location = @Location,
                    contact_info = @ContactInfo,
                    certifications = @Certifications
                WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", farm.Name);
            cmd.Parameters.AddWithValue("@OwnerName", (object?)farm.OwnerName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Location", (object?)farm.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactInfo", (object?)farm.ContactInfo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Certifications", (object?)farm.Certifications ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using SqlConnection conn = _dbHelper.GetConnection();
            const string sql = "DELETE FROM dbo.farms WHERE id = @Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Farm Map(SqlDataReader reader)
        {
            return new Farm
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"].ToString()!,
                OwnerName = reader["owner_name"] == DBNull.Value ? null : reader["owner_name"].ToString(),
                Location = reader["location"] == DBNull.Value ? null : reader["location"].ToString(),
                ContactInfo = reader["contact_info"] == DBNull.Value ? null : reader["contact_info"].ToString(),
                Certifications = reader["certifications"] == DBNull.Value ? null : reader["certifications"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["created_at"])
            };
        }
    }
}


